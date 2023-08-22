require(mesh);
require(GCModeller);
require(mzkit);
require(graphics);
require(graphics2D);

imports "clustering" from "MLkit";

imports "mzweb" from "mzkit";

setwd(@dir);

sink(file = "./run.txt");

let mesh = mesh(
    mass.range = [50, 2000], 
    feature.size = 1000, 
    mzdiff = 0.005);
let raster = as.raster(readImage(`../../docs\single-cells-have-their-own-defenses-against-pathogens3.jpg`)); 
let labels = raster_vec(raster);
let gmm = clustering::gmm(labels, components = 3);
let gauss = gmm.predict_proba(gmm);

print(labels);

labels = gmm.predict(gmm);

print(gauss, max.print = 6);
print(labels);

write.csv(gauss, file = "./gauss.csv", row.names = TRUE);

bitmap(file = "./raster1.png", size = [1920, 1080]);
rasterHeatmap(raster);
dev.off();

samples.raster(mesh, raster, label = labels);

let pack = mesh::expr1(mesh, mzpack = TRUE, spatial = TRUE);

write.mzPack(pack, file = `demo_singlecells-small.mzPack`, version = 2);

sink();