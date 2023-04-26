

dbpfFile.Seek(this.chunkOffset + idx.offset, SeekOrigin.Begin);
                                cFileSize = readFile.ReadInt32();
                                log.MakeLog("cFileSize: " + cFileSize, true);
                                cTypeID = readFile.ReadUInt16().ToString("X4");
                                log.MakeLog("cTypeID: " + cTypeID, true);
                                if (cTypeID == "FB10"){
                                    byte[] tempBytes = readFile.ReadBytes(3);
                                    uint cFullSize = ReadEntries.QFSLengthToInt(tempBytes);
                                    string cpfTypeID = readFile.ReadUInt32().ToString("X8");
                                    log.MakeLog("CPF Type is: " + cpfTypeID, true);
                                    if((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")){
                                        //actual CPF
                                        //infovar = readentries.readCPFchunk(readFile);
                                        log.MakeLog("Dropped into the first if CPFtypeID is with " + cpfTypeID, true);
                                    } else {
                                        log.MakeLog("Dropped into the first CPFtypeID else with " + cpfTypeID, true);
                                        dbpfFile.Seek(this.chunkOffset + idx.offset + 9, SeekOrigin.Begin);
                                        DecryptByteStream decompressed = new DecryptByteStream(ReadEntries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));
                                        var allstream = decompressed.GetEntireStream();
                                        var stringfromstream = Encoding.UTF8.GetString(allstream);
                                        log.MakeLog("Second CPF type id is: " + cpfTypeID, true);
                                        if (cpfTypeID == "E750E0E2") 
                                        {

                                            // Read first four bytes
                                            //cpfTypeID = decompressed.ReadUInt32().ToString("X8");
                                            log.MakeLog("Dropped into IF: " + cpfTypeID, true);

                                            if ((cpfTypeID == "CBE7505E") || (cpfTypeID == "CBE750E0")) 
                                            {
                                                log.MakeLog("Dropped into another IF: " + cpfTypeID, true);
                                                // Is an actual CPF file, so we have to decompress it...
                                                //infovar = readentries.readCPFchunk(decompressed);

                                            }

                                        } 
                                        else 
                                        {
                                            log.MakeLog("Ended up in Else: " + cpfTypeID, true);
                                            //infovar = readentries.readXMLchunk(decompressed);
                                        }
                                    }
                                }




                                
								
								
								
								
if (typefound == "STR#") {
                                log.MakeLog("Found STR#.", true);
                                foreach (indexEntry idx in indexData){
                                    if (idx.typeID == "53545223") {                                    
                                        dbpfFile.Seek(chunkOffset + idx.offset, SeekOrigin.Begin);
                                        int cFileSize = readFile.ReadInt32();
                                        string cTypeID = readFile.ReadUInt16().ToString("X4");
                                        if (cTypeID == "FB10") 
                                        {
                                            byte[] tempBytes = readFile.ReadBytes(3);
                                            uint cFullSize = ReadEntries.QFSLengthToInt(tempBytes);

                                            DecryptByteStream decompressed = new DecryptByteStream(ReadEntries.Uncompress(readFile.ReadBytes(cFileSize), cFullSize, 0));

                                            infovar = readentries.readSTRchunk(decompressed);
                                        } 
                                    } else {
                                        infovar = readentries.readSTRchunk(readFile);
                                    }
                                    thisPackage.Title = infovar.Title;
                                    thisPackage.Description = infovar.Description;
                                    log.MakeLog("infovar: " + infovar.Description, true);
                                    log.MakeLog("infovar: " + infovar.Title, true);
                                    if (infovar.Title != null) {
                                        log.MakeLog("Title is not null!", true);
                                        thisPackage.Title = infovar.Title;
                                        break;
                                    }
                                    if (infovar.Description != null) {
                                        log.MakeLog("Description is not null!", true);
                                        thisPackage.Description = infovar.Description;
                                        break;
                                    }
                                    
                                }
                                break;
                            } 