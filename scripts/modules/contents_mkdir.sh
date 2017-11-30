# command: ./script.bash alpha beta gamma
# Variables: $1=='alpha'; $2=='beta'; $3=='gamma'
# The variable $0 is the script's name. The total number of arguments is stored in $#. The variables $@ and $* return all the arguments.

# 脚本使用方法：
#
# ./contents_mkdir.sh directory_path
#
# 使用这个脚本进行蛋白组分析操作的工作区文件夹的初始化操作
#
root=$1

echo "Initialize report content directory at $root/";

mkdir -p "$root/1. samples/";
mkdir -p "$root/2. annotations/";
mkdir -p "$root/3. DEPs/";
mkdir -p "$root/4. functional_analysis/";

exit 0;
