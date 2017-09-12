# 这个脚本适用于iTraq类型的结果数据文件：
# 调用sciBASIC#工具从Excel表格之中读取表达数据到一个csv文件之中，
# 用作为数据分析的最开始的源头

# 这个脚本之中分两个步骤来进行
#
# 1. 从Excel文件之中导出csv文件
# 2. 移动Excel文件到sample文件夹，对原始数据进行归档操作
# 3. 移动矩阵文件到DEPs文件夹之中准备进入下一步的计算分析

      xlsx=$1;
   symbols=$2;
sampleInfo=$3;

   samples="1. samples";
        wd=`dirname "$xlsx"`;
  fileName=`basename "$xlsx"`;
       csv="$wd/$samples/$fileName.csv";

# 调用sciBASIC的Excel工具进行格式转换
Excel /extract.csv /in "$xlsx" /sheet "Sheet1" /out "$csv";

# 绘制'calc. pI' - 'MW [kDa]'散点图
eggHTS /plot.pimw /in "$csv" /legend.fontsize 20 /legend.size 100,30 /size 1600,1200 /pt.size 8 

# 解析出样本数据矩阵以及头部标签的替换，再将替换好的数据以及symbols标记对应信息写入原来的xlsx文件之中的第二张和第三张表之中
#
# /out 参数是一个文件夹路径
eggHTS /iTraq.Sign.Replacement /in "$csv" /symbols "$symbols" /out "$wd/$samples"

# 删除旧的原始数据，并备份最开始的原始数据$xlsx文件
rm -f "$csv"
yes | cp "$xlsx" "$wd/$samples/$fileName.xlsx"

# 经过头部的标记信息对用户样品标签信息替换之后所得到的新的原始数据
csv="$wd/$samples/$fileName.sample.csv";
xlsx="$wd/$samples/$fileName.xlsx";

# 将$csv和标记信息$symbols写入xlsx文件之中
Excel /push /write "$xlsx" /sheet "sample" /table "$csv"
Excel /push /write "$xlsx" /sheet "symbols" /table "$symbols" 

# 删除中间文件
rm -f "$csv"

# 根据sampleInfo文件将矩阵文件进行切割，并复制到DEPs/matrix文件夹之中
csv="$wd/$samples/matrix.csv";
eggHTS /iTraq.matrix.split /in "$csv" /sampleInfo "$sampleInfo" /out "$wd/3. DEPs/matrix/"

# 删除中间文件
rm -f "$csv"