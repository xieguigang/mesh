require(mesh);
require(GCModeller);
require(mzkit);
require(graphics);

imports "mzweb" from "mzkit";

let mesh = mesh(
    mass.range = [50, 2000], 
    feature.size = 100, 
    mzdiff = 0.005);
let raster = as.raster(readImage(`${@dir}/../../docs\single-cells-have-their-own-defenses-against-pathogens3.jpg`)); 

bitmap(file = "./raster1.png", size = [1920, 1080]);
rasterHeatmap(raster);
dev.off();

samples.raster(mesh, raster);

let pack = mesh::expr1(mesh, mzpack = TRUE, spatial = TRUE);

write.mzPack(pack, file = `${@dir}/demo_singlecells.mzPack`, version = 2);