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

let kegg_db = HDS::openStream("\GCModeller\src\repository\graphquery\kegg\tools\cache.db", readonly = TRUE);
let files = as.data.frame(HDS::files(kegg_db, 
dir = "/Metabolism/Energy metabolism/", recursive = TRUE));

# print(as.data.frame(files));
files = files[files$type == "dir", ];
files = files[basename(files$path) == "compounds", ];

# get all compounds
files = lapply(files$path, function(dir) {
    as.data.frame(HDS::files(kegg_db, dir = `${dir}/`, recursive = TRUE))$path;
}) |> unlist();

files = data.frame(files, name = basename(files));
files = files |> groupBy("name") |> sapply(function(d) {
    const list = (d$files);

    if (length(list) == 1) {
        list;
    } else {
        .Internal::first(list);
    }
});

print(files);

let compounds = lapply(files, function(path) {
    try({
        loadXml(HDS::getText(kegg_db, fileName = path), typeof = "kegg_compound");
    });
});

names(compounds) = basename(files);

# str(compounds);

# stop();

let mesh = mesh(
    mass.range = [50, 3000], 
    feature.size = 300, 
    mzdiff = 0.01);
let ruler = color.height_map(["#000000","#004b71","#2336cf","#0accfd","#9133ef","#b85997","#e79953","#e6ed76","#ffffff"]);
let raster = as.raster(
    img = readImage(`../../docs\Visualize-Metabolic-Process-at-the-Single-Cell-Level.png`),
    rgb.stack = ruler
); 

`<span style="background-color:${as.vector(ruler)}">&nbsp;</span>`
|> writeLines(con = `${@dir}/ruler.html`)
;

# stop();

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

mesh 
|> samples.raster(raster)
|> metabolites(metabolites = unlist(compounds),  adducts = ["[M+H]+","[M+Na]+","[M+K]+","[M+H-H2O]+","[M]+"])
;
# samples.raster(mesh, raster, label = labels);

let pack = mesh::expr1(mesh, mzpack = TRUE, spatial = TRUE, q= 0.3);

write.mzPack(pack, file = `demo_singlecells.mzPack`, version = 2);