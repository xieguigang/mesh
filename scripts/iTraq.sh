#!/bin/bash

# 对于iTraq和TMT这两种类型的蛋白组实验而言，数据处理的方式都是几乎一样的，
# 所以二者是公用同一套数据分析脚本的
#

# 进行命令行的帮助信息的提示
if [ $# -gt 0 ]; then

    # 取出所输入的文件路径

    # ===================================================================================
    # 脚本输入数据：
    #
    # 1. project.xlsx   iTraq质谱实验结果数据文件
    # 2. sampleInfo.csv 包括实验组别与样品之间的已对应关系的定义，以及一些绘图操作的时候的颜色和形状的定义
    # 3. design.csv     比对试验设计，用于生成log2FC值以进行差异表达蛋白的计算
    # 4. symbols.csv    将iTraq的标记替换为样品的标签
    # ===================================================================================

    # 1. 首先获取得到原始数据excel文件的路径
       project=$1;
    # 2. 实验的分组设计
    sampleInfo=$2;
    # 3. 进行比对实验设计得到差异表达蛋白所需要的比对信息
        design=$3;
    # 4. 以及iTraq实验的标记对用户样品名称的映射
       symbols=$4;

else
    # 用户没有输入任何命令行参数，则需要在这里打印出帮助信息

    # 但是在当前的工作区之中发现了所需求的文件，则会直接进入下一步
    allHave=true;
    
    project="./project.xlsx";    
    if [ !(-f "$project") ] then
        allHave=false;
    fi

    sampleInfo="./sampleInfo.xlsx";    
    if [ !(-f "$sampleInfo") ] then
        allHave=false;
    fi

    design="./design.xlsx";    
    if [ !(-f "$design") ] then
        allHave=false;
    fi

    symbols="./symbols.xlsx";    
    if [ !(-f "$symbols") ] then
        allHave=false;
    fi

    if [allhave = false] then

        # 用户没有输入任何命令行参数，则需要在这里打印出帮助信息
        # 脚本调用
        #
        # iTraq.sh ./project.xlsx ./sampleInfo.csv ./design.csv ./symbols.csv

        echo "Usage:\n";
        echo "\n";
        echo "    iTraq.sh ./project.xlsx ./sampleInfo.csv ./design.csv ./symbols.csv\n";
        echo "\n";
        echo "\n";
        echo "Where:\n";
        echo "\n";
        echo "             - project.xlsx:   Excel file that contains the iTraq source value\n";
        echo "             - sampleInfo.csv: Contains the information about the relationship between the sample label and experiment group,\n";
        echo "                               and the definition of the plot color and shapes in the legend.\n";
        echp "             - design.csv:     The experiment design that using for analysis of the DEPs result\n";
        echo "             - symbols.csv:    Relationsips between the iTraq label and the user sample label\n";

        exit 1;
    fi   
fi


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

enrichment=`reference "enrichment.sh"`;

network=`reference "string_network.sh"`;


# 调用自动报告脚本进行iTraq蛋白组数据分析报告的自动生成
./auto_reports/iTraq.sh "$work" "$sampleInfo" "$design" "$symbols"