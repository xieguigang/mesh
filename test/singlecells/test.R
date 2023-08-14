require(mesh);
require(GCModeller);
require(mzkit);
require(graphics);

imports "mzweb" from "mzkit";

let mesh = mesh(
    mass.range = [50, 2000], 
    feature.size = 2500, 
    mzdiff = 0.005);
let ratser = as.raster(readImage(`${@dir}/../../docs/Visualize-Metabolic-Process-at-the-Single-Cell-Level.png`)); 

samples.raster(mesh, raster);

let pack = mesh::expr1(mesh, mzpack = TRUE, spatial = TRUE);

write.mzPack(pack, file = `${@dir}/demo_singlecells.mzPack`, version = 2);