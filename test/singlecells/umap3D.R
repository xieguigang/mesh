imports ["dataset", "umap"] from "MLkit";

setwd(@dir);

let raw = read.csv("./ions.csv", check.modes = FALSE, check.names = FALSE, row.names = 1);

print(rownames(raw));
print(colnames(raw));

for(col in colnames(raw)) {
	raw[, col] = as.numeric(raw[, col]);
}

let manifold = raw
|> umap(
	dimension            = 3, 
	numberOfNeighbors    = 15,
    localConnectivity    = 1,
    KnnIter              = 64,
    bandwidth            = 1
)
;

let result = as.data.frame(manifold$umap, labels = manifold$labels, dimension = ["X", "Y", "Z"]);

write.csv(result, file = "./spot-scatter.csv");