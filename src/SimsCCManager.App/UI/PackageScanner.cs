using Godot;
using SimsCCManager.Containers;
using SimsCCManager.Debugging;
using SimsCCManager.Globals;
using SimsCCManager.PackageReaders;
using SimsCCManager.PackageReaders.Containers;
using SimsCCManager.Packages.Containers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Serialization;

public partial class PackageScanner : Control
{
	public delegate void PackageScannedEvent(string id);
	public PackageScannedEvent PackageScanned;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	public void Scan(SimsPackage package, Sims2Instance sims2Instance = null, Sims3Instance sims3Instance = null, Sims4Instance sims4Instance = null){
		new Thread(() => {
			if (package.Game == Games.Sims2){
				Sims2PackageReader s2pr = new();
				s2pr.simsPackage = package;
				s2pr.ReadSims2Package();
			} else if (package.Game == Games.Sims3){

			} else if (package.Game == Games.Sims4){
				Sims4PackageReader s4pr = new();
				s4pr.SearchS4Package(new FileInfo(package.Location), sims4Instance);
			}
			package = CheckOverrides(package);
			CallDeferred(nameof(EmitScanned), package.Identifier.ToString());
		}){IsBackground = true}.Start();
	}

	private SimsPackage CheckOverrides(SimsPackage package){
		package = Utilities.LoadPackageFile(package);
		List<OverriddenList> OverridesList = new();
		XmlSerializer packageSerializer = new XmlSerializer(typeof(List<OverriddenList>));		
		if (package.Game == Games.Sims2){
			Sims2ScanData scandata = (Sims2ScanData)package.ScanData;
			string datafolder = Path.Combine(GlobalVariables.InstallDirectory, @"data\Sims2");
			foreach (string file in Directory.GetFiles(datafolder)){
				using (FileStream fileStream = new(file, FileMode.Open, System.IO.FileAccess.Read)){
                    using (StreamReader streamReader = new(fileStream)){
                        OverridesList = (List<OverriddenList>)packageSerializer.Deserialize(streamReader);
                        streamReader.Close();
                    }
                    fileStream.Close();
                }
				foreach (string iid in scandata.FullIDs){
					if (OverridesList.Where(x => x.Override == iid).Any()){
						scandata.Override = true;
						package.Override = true;
						scandata.Overrides.Add(OverridesList.Where(x => x.Override == iid).First());						
					}
				}
			}
			package.ScanData = scandata;
			if (((Sims2ScanData)package.ScanData).Overrides.Any()){
				if (GlobalVariables.DebugMode) Logging.WriteDebugLog(string.Format("Package {0} is an override. Overridden resource: {1}", package.FileName, OverriddenList.OverridesListToString(((Sims2ScanData)package.ScanData).Overrides)));		
				Sims2ScanData scan = (Sims2ScanData)package.ScanData;
				scan.Overrides = scan.Overrides.GroupBy(p => p.Override)
				.Select(g => g.First())
				.ToList();
				package.ScanData = scan;
				package.ScanData.Override = true;
				package.Override = true;
				package.WriteInfoFile();
			}			
		}
		return package;				
	}

	private void EmitScanned(string identifier){
		PackageScanned.Invoke(identifier);
		//EmitSignal("PackageScanned", identifier);
	}
}
