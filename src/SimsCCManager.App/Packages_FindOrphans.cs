using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SimsCCManager.Packages.Containers;
using SQLiteNetExtensions.Extensions;
using SSAGlobals;

namespace SimsCCManager.Packages.Orphans
{
    public class FindOrphans
    {
        LoggingGlobals log = new LoggingGlobals();

        public void FindMatches(List<SimsPackage> packages){
            List<SimsPackage> s2 = packages.Where(p => p.Game == 2).ToList();
            List<SimsPackage> s3 = packages.Where(p => p.Game == 3).ToList();
            List<SimsPackage> s4 = packages.Where(p => p.Game == 4).ToList();
            FindMatchesS2(s2);
            FindMatchesS3(s3);
            FindMatchesS4(s4);
        }

        public void FindMatchesS2(List<SimsPackage> packages){            
            
        }
        public void FindMatchesS3(List<SimsPackage> packages){            
            
        }

        public void FindMatchesS4(List<SimsPackage> packages){
            List<SimsPackage> adjustedPackages = new();
            List<SimsPackage> todelete = new();

            var meshes = packages.Where(p => p.Mesh == true);
            var recolors = packages.Where(p => p.Recolor == true); 
            int meshesnum = meshes.Count();
            int recnum = recolors.Count();
            GlobalVariables.pairingNum = meshesnum + recnum;         

            foreach (SimsPackage mesh in meshes){
                bool adj = false;
                List<PackageMatchingRecolors> mrec = new();
                foreach (PackageMeshKeys meshKey in mesh.MeshKeys){
                    var split = meshKey.MeshKey.Split("-");
                    var matchingrecolors = recolors.Where(r => r.CASPartKeys.Any(x => x.CASPartKey.Contains(meshKey.MeshKey)) || r.OBJDPartKeys.Any(x => x.OBJDKey.Contains(meshKey.MeshKey)));
                    if (matchingrecolors.Any()){
                        adj = true;
                        todelete.Add(mesh);
                        todelete.AddRange(matchingrecolors);
                        foreach (SimsPackage package in matchingrecolors){
                            package.MatchingMesh = mesh.PackageName;
                            package.Orphan = false;
                            adjustedPackages.Add(package);
                            mrec.Add(new PackageMatchingRecolors(){ MatchingRecolor = package.PackageName });
                        }
                    } else if (GlobalVariables.S4_Overrides_All.Contains(split[2])){
                        mesh.Orphan = false;                    
                        adjustedPackages.Add(mesh);
                    }
                }
                if (adj == true){
                    mesh.Orphan = false;
                    mesh.MatchingRecolors = mrec;
                    adjustedPackages.Add(mesh);
                }
                GlobalVariables.pairingCount++;
            }
            GlobalVariables.DatabaseConnection.DeleteAll(todelete);
            GlobalVariables.DatabaseConnection.InsertAllWithChildren(adjustedPackages);
        }
    }
}