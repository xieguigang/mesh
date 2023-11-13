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


# str(compounds);

# stop();

let mesh = mesh(
    mass.range = [50, 2000], 
    feature.size = 500, 
    mzdiff = 0.005);
let ruler = color.height_map(["#000000","#004b71","#2336cf","#0accfd","#9133ef","#b85997","#e79953","#e6ed76","#ffffff"]);
let raster = as.raster(
    img = readImage("\demo\spatial\all.png")
    # rgb.stack = ruler
); 

# `<span style="background-color:${as.vector(ruler)}">&nbsp;</span>`
# |> writeLines(con = `${@dir}/ruler.html`)
# ;

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
|> metabolites(metabolites = unlist(compounds),  adducts = ["[M+H]+","[M+Na]+","[M+K]+","[M+NH4]+","[M+H-H2O]+"])
;
# samples.raster(mesh, raster, label = labels);

for(file in list.files("\demo\spatial\segments", pattern = "*.png")) {
    raster = as.raster(
        img = readImage(file)
        # rgb.stack = ruler
    ); 

    mesh <- samples.raster(mesh, raster, label = basename(file));
}


let pack = mesh::expr1(mesh, mzpack = TRUE, spatial = TRUE, q= 0.5);

write.mzPack(pack, file = `demo.mzPack`, version = 2);