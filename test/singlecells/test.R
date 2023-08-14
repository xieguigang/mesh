require(mesh);
require(GCModeller);
require(mzkit);
require(graphics);
require(graphics2D);

imports "mzweb" from "mzkit";

setwd(@dir);

let mesh = mesh(
    mass.range = [50, 2000], 
    feature.size = 100, 
    mzdiff = 0.005);
let raster = as.raster(readImage(`../../docs\Visualize-Metabolic-Process-at-the-Single-Cell-Level.png`)); 

bitmap(file = "./raster1.png", size = [1920, 1080]);
rasterHeatmap(raster);
dev.off();

samples.raster(mesh, raster);

let pack = mesh::expr1(mesh, mzpack = TRUE, spatial = TRUE);

write.mzPack(pack, file = `demo_singlecells_small.mzPack`, version = 2);