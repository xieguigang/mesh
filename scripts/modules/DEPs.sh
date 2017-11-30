

# 首先分别从命令行之中获取得到工作区文件夹和样本分组信息
# #wd文件夹就是DEPs/matrix文件夹，在这个文件夹之中保存着matrix数据
        wd=$1;
sampleInfo=$2;

# 获取得到上一级的父文件夹即/3.DEPs/文件夹作为DEPs计算分析和绘图操作的工作区目录
 workspace=`dirname "$wd"`;

# 创建一个空的excel文件
xlsx="$worksapce/T.test/sampleGroups.log2FC_t.test.xlsx";
Excel /create "$xlsx"

# 对DEPs/T.test/文件夹之中的每一个csv文件都做差异表达计算分析
for csv in "$workspace/T.test/*.csv"
do 
    
    # 进行T检验，并删除旧的中间文件
    eggHTS /iTraq.t.test /in "$csv" /level 1.5 /FDR
    rm -f "$csv"

    # 绘制火山图
    groupName=`basename "$csv"`;
    csv="$workspace/T.test/$groupName-log2FC_t.test.csv";
    eggHTS /vocano.plot /in "$csv"

    # 结果数据写入Excel文件之中
    Excel /push /write "$xlsx" /sheet "$groupName" /table "$csv" 

done

# 进行热图数据的合并获取以及绘制操作
eggHTS /heatmap /in "$workspace/T.test/" /out "$workspace/heatmap/"

# 进行文氏图数据的合并获取以及绘制操作
eggHTS /venn /in "$workspace/T.test/" /out "$workspace/venn/"

# 将中间数据移动到functional_analysis文件夹之中，准备进行富集分析
# 其实富集分析只需要有基因编号列表即可，但是因为KEGG绘制代谢网络图和蛋白互作网络的绘制
# 还需要结合蛋白的表达量的信息，所以在这里就直接将中间文件给移动过去了
functional_analysis=`dirname "$workspace"`;
functional_analysis="$functional_analysis/4. functional_analysis/";

for csv in "$workspace/T.test/*.csv"
do     
    mv -f "$csv" "$functional_analysis"
done

exit 0;