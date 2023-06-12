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
        public SimsPackage FindMatchesS2(SimsPackage thisPackage){
            
            
            
            return thisPackage;
        }

        public SimsPackage FindMatchesS4(SimsPackage thisPackage){
            if (thisPackage.Recolor == true && thisPackage.Mesh == false){
                if (thisPackage.CASPartKeys.Any()){
                    foreach (var rec in thisPackage.CASPartKeys){
                        if (rec != "000000-000000-0000000000000000"){
                            string[] split = rec.Split('-');
                            var foundoverrides = (from overrides in GlobalVariables.S4OverridesList
                                                    where overrides.InstanceID == split[2]
                                                    select overrides).ToList();
                            if (foundoverrides.Any()){
                                thisPackage.Orphan = false;
                                return thisPackage;
                            }
                        }
                    }
                }
                
            }
            if (thisPackage.Mesh == true){
                foreach (var meshkey in thisPackage.MeshKeys){
                    InstancesMeshesS4 Mesh = new();
                    var MatchingRecolors = (from r in GlobalVariables.S4InstancesCacheRecolors
                                where thisPackage.CASPartKeys.Any(mr => r.Key == mr) && thisPackage.PackageName == r.PackageName
                                select r).ToList();
                    //see if the mesh is already listed somewhere
                    var MatchingMeshes = (from r in GlobalVariables.S4InstancesCacheMeshes
                                where thisPackage.CASPartKeys.Any(mr => r.Key == mr)
                                select r).ToList();
                    if (MatchingMeshes.Any()){
                        Mesh = MatchingMeshes[0];
                    } else {
                        Mesh.Key = meshkey;
                        Mesh.PackageName = thisPackage.PackageName;
                    }
                    if (thisPackage.CASPartKeys.Any()){
                        foreach (var rec in thisPackage.CASPartKeys){
                            if (rec != "000000-000000-0000000000000000"){
                                Mesh.MatchingRecolors.Add(new InstancesRecolorsS4(){
                                    Key = rec,
                                    PackageName = thisPackage.PackageName
                                });
                            }     
                        }
                    }                
                    if (MatchingRecolors.Any()){
                        thisPackage.Orphan = false;
                        foreach (var rec in MatchingRecolors){
                            if (!Mesh.MatchingRecolors.Contains(rec)){
                                Mesh.MatchingRecolors.Add(rec);  
                            }                                             
                        }                    
                        foreach (var rec in MatchingRecolors){
                            if (rec.MeshKey != null){
                                InstancesRecolorsS4 ir4 = new();
                                ir4 = rec;
                                ir4.MatchingMesh = Mesh; 
                                GlobalVariables.InstancesRecolorsS4Col.Enqueue(ir4);
                            }                        
                        }
                    }                
                    GlobalVariables.InstancesMeshesS4Col.Enqueue(Mesh);
                }
                
            } else if (thisPackage.Recolor == true){
                List<InstancesRecolorsS4> Recolors = new();
                var MatchingRecolors = (from r in GlobalVariables.S4InstancesCacheRecolors
                            where thisPackage.CASPartKeys.Any(mr => r.Key == mr) && thisPackage.PackageName == r.PackageName
                            select r).ToList();
                //see if the mesh is already listed somewhere
                var MatchingMeshes = (from r in GlobalVariables.S4InstancesCacheMeshes
                            where thisPackage.CASPartKeys.Any(mr => r.Key == mr)
                            select r).ToList();
                InstancesMeshesS4 Mesh = new();
                if (MatchingMeshes.Any()){    
                    thisPackage.Orphan = false;                                   
                    Mesh = MatchingMeshes[0];
                    foreach (var m in Mesh.MatchingRecolors){
                        Recolors.Add(new InstancesRecolorsS4{
                            MatchingMesh = Mesh,                            
                            Key = m.Key,
                            PackageName = m.PackageName
                        });
                    }
                    foreach (var mesh in MatchingMeshes){
                        InstancesMeshesS4 is4 = new();
                        is4 = mesh;
                        is4.MatchingRecolors.Clear();
                        foreach (var r in Recolors){
                            is4.MatchingRecolors.Add(r);
                        }
                        GlobalVariables.InstancesMeshesS4Col.Enqueue(is4);
                    }     
                }                
                if (thisPackage.CASPartKeys.Any()){
                    foreach (var rec in thisPackage.CASPartKeys){
                        if (rec != "000000-000000-0000000000000000"){
                            InstancesRecolorsS4 irs4 = new(){
                                Key = rec,
                                PackageName = thisPackage.PackageName
                            };
                            if (MatchingMeshes.Any()){
                                irs4.MatchingMesh = Mesh;
                            }
                            Recolors.Add(irs4);
                        }     
                    }
                }
                foreach (var r in Recolors){
                    GlobalVariables.InstancesRecolorsS4Col.Enqueue(r);   
                }                             
            }
            return thisPackage;
        }
    }
}