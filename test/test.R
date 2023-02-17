require(mesh);
require(GCModeller);

#' GCModeller DEG experiment analysis designer toolkit
imports "sampleInfo" from "phenotype_kit";
#' the gene expression matrix data toolkit
imports "geneExpression" from "phenotype_kit";

setwd(@dir);

let data = mesh()
|> mesh::samples(sampleinfo = read.sampleinfo(file = "./sampleInfo.csv",
    tsv = FALSE,
    exclude.groups = NULL,
    id.makenames = FALSE))
|> mesh::expr1()
;

write.expr_matrix(data, file = "./expr1.csv",
    id = "ions",
    binary = FALSE);

write.mzPack(as.mzPack(data), file = "./expr1.mzPack",
    version = 2);