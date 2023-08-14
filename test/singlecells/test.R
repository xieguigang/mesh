require(mesh);
require(GCModeller);
require(mzkit);

imports "mzweb" from "mzkit";

let mesh = mesh(
    mass.range = [50, 2000], 
    feature.size = 2500, 
    mzdiff = 0.005);

samples.spatial(mesh, x = 1:256, y = 1:200);

let pack = mesh::expr1(mesh, mzpack = TRUE, spatial = TRUE);

write.mzPack(pack, file = `${@dir}/demo_singlecells.mzPack`, version = 2);