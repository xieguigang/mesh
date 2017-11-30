#!/usr/bin/env python3

import os;
import os.path;
import shutil;
import subprocess;

from os import listdir;
from os.path import isfile, join;

# 蛋白组的DEPs计算分析以及火山图，文氏图和热图的绘制工具
#
# 1. 首先获取一个文件夹，这个文件夹包含有从sample文件之中根据sampleInfo分割出来的比对组别对应的表达值矩阵文件
# 2. 对每一个矩阵文件进行计算得到差异表达蛋白，并作出相应的数量上的统计
# 3. 之后对差异蛋白计算结果进行火山图的绘制
# 4. 之后将差异蛋白计算结果进行热图的绘制以及相应的kmeans聚类分析
# 5. 之后再进行文氏图的绘制操作

def pipeline(matrix_directory, log2FC_level=1.5, pvalue=0.05, FDR=0.05):
    '''
    @param matrix_directory: 保存有来自于上一部分模块调用得到的样品表达数据的矩阵csv文件的文件夹
    @param log2FC_level:     差异表达基因的最小的FoldChange的变化倍数，默认是1.5倍
    @param pvalue:           学生氏T检验成立所需要的pvalue阈值，默认是0.05
    @param FDR:              假阳性检验的阈值，默认是0.05

    请注意，当pvalue=1或者FDR=1的时候表示将不进行相应的统计学检验工作
    '''

    # 首先遍历matrix_directory文件夹中的每一个csv矩阵文件，
    # 然后对找到的所有的csv文件调用eggHTS程序进行T检验分析
    #
    # 为了程序能够重复允许调试，新生成的数据文件不会污染原始的数据文件，
    # 在这里将结果文件存放到t.test文件夹之中
    out_directory = "%s/t.test" % (os.path.dirname(matrix_directory));

    if os.path.isdir(out_directory):

        # 如果目标文件夹存在的话，则删除掉该文件夹，以防止数据污染
        # 清理工作区
        shutil.rmtree(out_directory);

    # 建立文件输出所需要的工作区
    os.makedirs(out_directory, exist_ok=True);

    t_test(
        log2FC_level=log2FC_level,
        pvalue=pvalue,
        FDR=FDR,
        matrix_directory=matrix_directory,
        out_directory=out_directory);

    return;


def t_test(matrix_directory, out_directory, log2FC_level=1.5, pvalue=0.05, FDR=0.05):
    '''
    @param matrix_directory: 保存有来自于上一部分模块调用得到的样品表达数据的矩阵csv文件的文件夹
    @param out_directory:    原始的矩阵数据完成T检验之后的输出文件夹
    @param log2FC_level:     差异表达基因的最小的FoldChange的变化倍数，默认是1.5倍
    @param pvalue:           学生氏T检验成立所需要的pvalue阈值，默认是0.05
    @param FDR:              假阳性检验的阈值，默认是0.05

    请注意，当pvalue=1或者FDR=1的时候表示将不进行相应的统计学检验工作
    '''
    
    CLI_template = 'eggHTS /iTraq.t.test /in "{0}" /level {1} /p.value {2} /FDR {3} /out "{4}"';

    # 获取matrix_directory输入文件夹之中的所有的csv文件
    matrix_csv_list = get_all_csv(matrix_directory);

    for csv in matrix_csv_list:

        # 开始在这里构建命令行调用，执行GCModeller之中的eggHTS蛋白组分析程序
        input  = "{}/{}".format(matrix_directory, csv);
        output = "{}/{}".format(out_directory, csv);
        CLI    = CLI_template.format(input, log2FC_level, pvalue, FDR, output);

        # 运行计算分析程序
        subprocess.check_output(CLI, shell=True);

    return;

def get_all_csv(directory):
    '''
	@param directory: 需要获取所有的csv文件的目录路径
	'''

    suffix = '.csv';
    matrix_csv_list = [
        filename

        for filename in listdir(directory)
        if isfile(join(directory, filename)) and filename.endswith(suffix)
    ];
    
    return matrix_csv_list;	

def volcano_plot(t_test_directory, log2FC_level = 1.5, pvalue = 0.05):
    '''
	@param t_test_directory: 来自于t_test函数调用所生成的T检验结果文件夹
	@param log2FC_level:     定义火山图之中的X轴的参考线的位置
	@param pvalue:           定义火山图之中的Y轴的参考线的位置
	'''

    # 在这里并不指定out文件了，eggHTS分析程序会自动在输入文件相同的文件夹下生成火山图
    CLI_template = '/DEP.logFC.Volcano /in {0} /p.value {1} /level {2} /size 1400,1400';

    # 获取t_test_directory输入文件夹之中的所有的csv文件
    matrix_csv_list = get_all_csv(t_test_directory);

    for csv in matrix_csv_list:

        # 开始在这里构建命令行调用，执行GCModeller之中的eggHTS蛋白组分析程序
        input  = "{}/{}".format(t_test_directory, csv);        
        CLI    = CLI_template.format(input, pvalue, log2FC_level);

        # 运行计算分析程序
        subprocess.check_output(CLI, shell=True);

    return;