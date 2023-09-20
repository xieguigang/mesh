require(mesh);
require(GCModeller);
require(mzkit);
require(graphics);
require(graphics2D);
require(HDS);

imports "mzweb" from "mzkit";
imports "clustering" from "MLkit";
imports "dataset" from "MLkit";

setwd(@dir);

const kegg_db = HDS::openStream("\GCModeller\src\repository\graphquery\kegg\tools\cache.db", readonly = TRUE);
const files = HDS::files(kegg_db);

print(files);

stop();

let mesh = mesh(
    mass.range = [50, 2000], 
    feature.size = 1000, 
    mzdiff = 0.005);
let raster = as.raster(readImage(`../../docs\Visualize-Metabolic-Process-at-the-Single-Cell-Level.png`)); 

# let labels = raster_vec(raster);
# let gmm = clustering::gmm(labels, components = 6);
# let gauss = gmm.predict_proba(gmm);

# print(labels);

# labels = gmm.predict(gmm);
# # labels = q_factors(labels, levels = 3);

# print(labels);

bitmap(file = "./raster1.png", size = [1920, 1080]);
rasterHeatmap(raster);
dev.off();

samples.raster(mesh, raster);
# samples.raster(mesh, raster, label = labels);

let pack = mesh::expr1(mesh, mzpack = TRUE, spatial = TRUE, q= 0.5);

write.mzPack(pack, file = `demo_singlecells.mzPack`, version = 2);