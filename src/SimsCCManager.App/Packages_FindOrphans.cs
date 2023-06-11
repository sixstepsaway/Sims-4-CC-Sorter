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
            if (thisPackage.Recolor == true && thisPackage.Mesh == false && thisPackage.Override == false){
                //if the package has a texture but no mesh
                var indb = (from r in GlobalVariables.S4InstancesCacheRecolors
                            where thisPackage.CASPartKeys.Any(mr => r.Key == mr) && thisPackage.PackageName == r.PackageName
                            select r).ToList();
                //see if the mesh is already listed somewhere
                var inmdb = (from r in GlobalVariables.S4InstancesCacheMeshes
                            where thisPackage.CASPartKeys.Any(mr => r.Key == mr)
                            select r).ToList();

                if (inmdb.Any()){
                    thisPackage.Orphan = false;
                    if (indb.Any()){
                        Parallel.ForEach(thisPackage.CASPartKeys, key => {
                            var meshquery = GlobalVariables.InstancesCacheConnection.Query<InstancesMeshesS4>(string.Format("SELECT * FROM Sims4Meshes where Key='{0}'", key));
                            var caspquery = GlobalVariables.InstancesCacheConnection.Query<InstancesRecolorsS4>(string.Format("SELECT * FROM Sims4Recolors where Key='{0}'", key));                            
                            if (meshquery.Any()){
                                Parallel.ForEach(caspquery, rc => {
                                    if (rc.MatchingMesh != key){
                                        rc.MatchingMesh = meshquery[0].PackageName;
                                        lock(GlobalVariables.InstancesRecolorsS4Col){
                                           GlobalVariables.InstancesRecolorsS4Col.Add(rc); 
                                        }                                                                                
                                    }
                                });
                            }                            
                            Parallel.ForEach(meshquery, mesh => {
                                if (!mesh.MatchingRecolors.Contains(thisPackage.PackageName)){
                                    mesh.MatchingRecolors.Add(thisPackage.PackageName);
                                    lock(GlobalVariables.InstancesMeshesS4Col){
                                        GlobalVariables.InstancesMeshesS4Col.Add(mesh);   
                                    }                                      
                                }
                            });
                        });
                    } else {
                        Parallel.ForEach(thisPackage.CASPartKeys, key => {
                            var meshquery = GlobalVariables.InstancesCacheConnection.Query<InstancesMeshesS4>(string.Format("SELECT * FROM Sims4Meshes where Key='{0}'", key));
                            if (meshquery.Any()){                                
                                Parallel.ForEach(meshquery, mesh => {
                                    if (!mesh.MatchingRecolors.Contains(thisPackage.PackageName)){
                                        mesh.MatchingRecolors.Add(thisPackage.PackageName);
                                        lock(GlobalVariables.InstancesMeshesS4Col){
                                            GlobalVariables.InstancesMeshesS4Col.Add(mesh);   
                                        }                                           
                                    }
                                });
                            }
                        });
                    }
                } else if (indb.Any()){
                    log.MakeLog(string.Format("{0}'s key is already in the database.", thisPackage.PackageName), true);
                } else {
                    log.MakeLog(string.Format("{0}'s key is not in the database, so we're adding it.", thisPackage.PackageName), true);
                    Parallel.ForEach(thisPackage.CASPartKeys, cpk => {
                        InstancesRecolorsS4 icr = new(){
                            PackageName = thisPackage.PackageName,
                            Key = cpk
                        };
                        lock(GlobalVariables.InstancesRecolorsS4Col){
                            GlobalVariables.InstancesRecolorsS4Col.Add(icr); 
                        }                 
                    });
                }
            }
            if (thisPackage.Recolor == true && thisPackage.Mesh == true && thisPackage.Override == false){
                thisPackage.Orphan = false;
                //if the package has both a mesh and texture-- 
                //check if the database has any of the keys for the texture
                var indb = (from r in GlobalVariables.S4InstancesCacheRecolors
                            where thisPackage.CASPartKeys.Any(mr => r.Key == mr) && thisPackage.PackageName == r.PackageName
                            select r).ToList();
                if (indb.Any()){
                    log.MakeLog(string.Format("{0}'s key is already in the database.", thisPackage.PackageName), true);
                } else {
                    log.MakeLog(string.Format("{0}'s key is not in the database, so we're adding it.", thisPackage.PackageName), true);
                    Parallel.ForEach(thisPackage.CASPartKeys, cpk => {
                        InstancesRecolorsS4 icr = new(){
                            PackageName = thisPackage.PackageName,
                            Key = cpk,
                            MatchingMesh = thisPackage.PackageName
                        };
                        lock(GlobalVariables.InstancesRecolorsS4Col){
                            GlobalVariables.InstancesRecolorsS4Col.Add(icr); 
                        }
                    });
                }
                //check if the database has the key for the mesh
                var indbr = (from r in GlobalVariables.S4InstancesCacheMeshes
                            where thisPackage.MeshKeys.Any(mr => r.Key == mr) && thisPackage.PackageName == r.PackageName
                            select r).ToList();
                if (indbr.Any()){
                    log.MakeLog(string.Format("{0}'s key is already in the database.", thisPackage.PackageName), true);
                    //add this recolor to the mesh listing
                    Parallel.ForEach(thisPackage.MeshKeys, key => {
                        var meshquery = GlobalVariables.InstancesCacheConnection.Query<InstancesMeshesS4>(string.Format("SELECT * FROM Sims4Meshes where Key='{0}'", key));
                        InstancesRecolorsS4 icr = new(){
                            PackageName = thisPackage.PackageName,
                            Key = key
                        };

                        Parallel.ForEach(meshquery, mesh => {
                            if (!mesh.MatchingRecolors.Contains(thisPackage.PackageName)){
                                mesh.MatchingRecolors.Add(thisPackage.PackageName);
                                lock(GlobalVariables.InstancesMeshesS4Col){
                                    GlobalVariables.InstancesMeshesS4Col.Add(mesh);
                                }
                                icr.MatchingMesh = mesh.PackageName;
                            }
                            lock(GlobalVariables.InstancesRecolorsS4Col){
                                GlobalVariables.InstancesRecolorsS4Col.Add(icr); 
                            }
                        });
                    });
                } else {
                    log.MakeLog(string.Format("{0}'s key is not in the database, so we're adding it.", thisPackage.PackageName), true);
                    Parallel.ForEach(thisPackage.MeshKeys, cpk => {
                        InstancesMeshesS4 icr = new(){
                            PackageName = thisPackage.PackageName,
                            Key = cpk
                        };
                        icr.MatchingRecolors.Add(thisPackage.PackageName);
                        lock(GlobalVariables.InstancesMeshesS4Col){
                            GlobalVariables.InstancesMeshesS4Col.Add(icr);   
                        }                          
                    });  
                }
            }
            if (thisPackage.Mesh == true && thisPackage.Override == false ){
                //if the package has a mesh
                List<InstancesCacheMeshes> inmdb = (from r in GlobalVariables.S4InstancesCacheMeshes
                            where thisPackage.MeshKeys.Any(mr => r.Key == mr) && thisPackage.PackageName == r.PackageName
                            select r).ToList();                            
                List<InstancesCacheRecolors> indb = (from r in GlobalVariables.S4InstancesCacheRecolors
                            where thisPackage.CASPartKeys.Any(mr => r.Key == mr) && thisPackage.PackageName == r.PackageName
                            select r).ToList();
                if (inmdb.Any()){
                    log.MakeLog(string.Format("{0}'s key is already in the database.", thisPackage.PackageName), true);
                    if (indb.Any()){
                        thisPackage.Orphan = false;
                        Parallel.ForEach(thisPackage.MeshKeys, key => {
                            var meshquery = GlobalVariables.InstancesCacheConnection.
                            Query<InstancesMeshesS4>(string.Format("SELECT * FROM Sims4Meshes where Key='{0}'", key));
                            var caspquery = GlobalVariables.InstancesCacheConnection.Query<InstancesRecolorsS4>(string.Format("SELECT * FROM Sims4Recolors where Key='{0}'", key));
                            InstancesMeshesS4 icr = new(){
                                PackageName = thisPackage.PackageName,
                                Key = key
                            };

                            if (caspquery.Any()){
                                Parallel.ForEach(caspquery, rc => {
                                    if (rc.MatchingMesh != key){
                                        rc.MatchingMesh = thisPackage.PackageName;lock(GlobalVariables.InstancesRecolorsS4Col){
                                           GlobalVariables.InstancesRecolorsS4Col.Add(rc); 
                                        }
                                        icr.MatchingRecolors.Add(rc.PackageName);
                                    }
                                });
                            }
                            lock(GlobalVariables.InstancesMeshesS4Col){
                                GlobalVariables.InstancesMeshesS4Col.Add(icr);   
                            }                               
                        });
                    }
                } else {
                    log.MakeLog(string.Format("{0}'s key is not in the database, so we're adding it.", thisPackage.PackageName), true);
                    List<InstancesMeshesS4> meshlist = new();
                    Parallel.ForEach(thisPackage.MeshKeys, cpk => {
                        InstancesMeshesS4 icr = new(){
                            PackageName = thisPackage.PackageName,
                            Key = cpk
                        };
                        lock(GlobalVariables.InstancesMeshesS4Col){
                            GlobalVariables.InstancesMeshesS4Col.Add(icr);   
                        }                                        
                    });
                }
            }        
            return thisPackage;
        }
    }
}