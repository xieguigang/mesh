# 对于iTraq和TMT这两种类型的蛋白组实验而言，数据处理的方式都是几乎一样的，
# 所以二者是公用同一套数据分析脚本的
#
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

# 首先获取得到原始数据excel文件的路径
project=$1;

# 然后使用公用脚本解析出matrix
extract_matrix="$getwd/modules/extract_matrix.sh";