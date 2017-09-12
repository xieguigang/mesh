# 对于iTraq和TMT这两种类型的蛋白组实验而言，数据处理的方式都是几乎一样的，
# 所以二者是公用同一套数据分析脚本的
#

# 因为考虑到需要引用其他的公用的bash脚本模块
# 所以在这里需要得到当前的脚本所处的文件夹的绝对路径
file_name=`basename $0`;
    getwd="";

if [ "`echo $0 | cut -c1`" = "/" ]; then
  getwd=`dirname $0`;
else
  getwd=`pwd`/`echo $0 | sed -e s/$file_name//`;
fi

echo "getwd() => $getwd";

#
# 函数获取得到对modules文件夹下的子模块的引用路径
#
function reference() {
    local file_name=$1;
    echo "$getwd/modules/$file_name";
}

# 首先获取得到原始数据excel文件的路径
   project=$1;
# 以及iTraq实验的标记对用户样品名称的映射
   symbols=$2;
# 最后则是实验的分组设计
sampleInfo=$3;

# 初始化文件目录
# 得到$project文件的父目录作为工作区目录进行初始化
     file_name=`basename $project`;
          work=`echo $project | sed -e s/$file_name//`;
contents_mkdir=`reference "contents_mkdir.sh"`;

# 执行脚本命令进行工作区的初始化操作
contents_mkdir "$work";

# 然后使用公用脚本解析出matrix
extract_matrix=`reference "extract_matrix.sh"`;

# 执行脚本命令进行matrix的释出和原始数据文件在工作区内的移动
# $project 提供原始数据的来源excel文件
# $symbols 提供itraq标记到用户样品标签的映射信息，用于提取出矩阵信息
# $sampleInfo 则是根据分组信息进行矩阵的分割与解析，从而能够进入下一个流程进行DEPs的计算分析
extract_matrix "$project" "$symbols" "$sampleInfo";

DEPs=`reference "DEPs.sh"`;

# 进行DEP的计算分析以及火山图，文氏图和热图的绘制操作
# $sampleInfo文件的所用是遍历其中的分组信息，分别对每一个分组的计算结果创建自己的工作区文件夹
DEPs "$work/3. DEPs/matrix/" "$sampleInfo";
