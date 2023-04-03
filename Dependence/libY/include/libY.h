#ifndef __LIBY_H__
#define __LIBY_H__

#include <stdlib.h>
#include <stdio.h>
#include <string.h>
#include <stdint.h>

#if (defined(Y_WIN32)) || (defined(Y_MINGW))
#include <Windows.h>
#include <WinBase.h>
#elif (defined(Y_UNIX)) || (defined(Y_MSYS))
#include <pthread.h>
#include <semaphore.h>
#endif

// 定义导出和导入符号
// 注意，如果是链接静态库，那么不需要__declspec(dllimport)
#if (defined(Y_WIN32))
    #ifdef Y_EXPORT
        #define YAPI __declspec(dllexport)
    #else
        #define YAPI __declspec(dllimport)
    #endif
#elif (defined(Y_WIN32))
    #ifdef Y_EXPORT
        #define YAPI __declspec(dllexport)
    #else
        #define YAPI
    #endif
#else
    #define YAPI
#endif

// Yerrno
#ifndef YERRNO
#define YERRNO
    #define YERR_SUCCESS						0
    #define YERR_FILE_NOT_FOUND                 1
    #define YERR_FAILED							2
    #define YERR_INVALID_JSON					3
    #define YERR_FILE_STAT_FAILED				4				// 读取文件属性失败
    #define YERR_VIDEO_FORMAT_NOT_SUPPORTED     5               // 不支持的视频格式
#endif

#ifndef YLOCK
#define YLOCK
    #if (defined(Y_WIN32)) || (defined(Y_MINGW))
    typedef CRITICAL_SECTION Ylock;
    #define Y_create_lock(Y_lock)             InitializeCriticalSection(&Y_lock)
    #define Y_delete_lock(Y_lock)             DeleteCriticalSection(&Y_lock)
    #define Y_lock_lock(Y_lock)               EnterCriticalSection(&Y_lock)
    #define Y_lock_unlock(Y_lock)             LeaveCriticalSection(&Y_lock)
    #elif (defined(Y_UNIX)) || (defined(Y_MSYS))
    typedef pthread_mutex_t Ylock;
    #define Y_create_lock(Y_lock)             pthread_mutex_init(&Y_lock, NULL)
    #define Y_delete_lock(Y_lock)             pthread_mutex_destroy(&Y_lock)
    #define Y_lock_lock(Y_lock)               pthread_mutex_lock(&Y_lock)
    #define Y_lock_unlock(Y_lock)             pthread_mutex_unlock(&Y_lock)
    #endif
#endif

/***********************************************************************************
 * @ file    : Ysem.h
 * @ author  : oheiheiheiheihei
 * @ version : 0.9
 * @ date    : 2022.02.26 10:45
 * @ brief   : 封裝不同平台下的信号量对象
 ************************************************************************************/
#ifndef __YSEM_H__
#define __YSEM_H__
    #if (defined(Y_WIN32)) || (defined(Y_MINGW))
    typedef HANDLE Ysem;
    #define Y_create_sem(Y_sem, max_size)       Y_sem = CreateSemaphoreW(NULL, 0, max_size, NULL)
    #define Y_delete_sem(Y_sem)                 ReleaseSemaphore(Y_sem, 0, NULL); CloseHandle(Y_sem)
    #define Y_sem_wait(Y_sem)                   WaitForSingleObject(Y_sem, INFINITE)
    #define Y_sem_post(Y_sem)                   ReleaseSemaphore(Y_sem, 1, NULL)
    #elif (defined(Y_UNIX)) || (defined(Y_MSYS))
    typedef sem_t Ysem;
    #define Y_create_sem(Y_sem, max_size)       sem_init(&Y_sem, 0, 0)
    #define Y_delete_sem(Y_sem)                 sem_destroy(&Y_sem)
    #define Y_sem_wait(Y_sem)                   sem_wait(&Y_sem)
    #define Y_sem_post(Y_sem)                   sem_post(&Y_sem)
    #endif
#endif


/***********************************************************************************
 * @ file    : Ylog.h
 * @ author  : oheiheiheiheihei
 * @ version : 0.9
 * @ date    : 2021.12.06 13:15
 * @ brief   : 一个简易的日志记录器，支持多线程
 * @ remark  : wprintf and printf cannot use in same env
 ************************************************************************************/
#ifndef YLOG
#define YLOG

	#define MAX_LOG_LINE        4096				// 字符个数，不管是宽字符还是多字节字符

	#define YLOGD2(logger, format, ...)		Y_log_write(logger, YLOG_LEVEL_DEBUG, __LINE__, format, __VA_ARGS__)
	#define YLOGI2(logger, format, ...)		Y_log_write(logger, YLOG_LEVEL_INFO, __LINE__, format, __VA_ARGS__)
	#define YLOGW2(logger, format, ...)		Y_log_write(logger, YLOG_LEVEL_WARN, __LINE__, format, __VA_ARGS__)
	#define YLOGE2(logger, format, ...)		Y_log_write(logger, YLOG_LEVEL_ERROR, __LINE__, format, __VA_ARGS__)

	#define YLOGD(format, ...)		Y_log_write(NULL, YLOG_LEVEL_DEBUG, __LINE__, format, ##__VA_ARGS__)
	#define YLOGI(format, ...)		Y_log_write(NULL, YLOG_LEVEL_INFO, __LINE__, format, ##__VA_ARGS__)
	#define YLOGW(format, ...)		Y_log_write(NULL, YLOG_LEVEL_WARN, __LINE__, format, ##__VA_ARGS__)
	#define YLOGE(format, ...)		Y_log_write(NULL, YLOG_LEVEL_ERROR, __LINE__, format, ##__VA_ARGS__)

	// 内部使用，调用者用不到这个枚举
	typedef enum Ylog_level_e
	{
		YLOG_LEVEL_DEBUG,
		YLOG_LEVEL_INFO,
		YLOG_LEVEL_WARN,
		YLOG_LEVEL_ERROR
	}Ylog_level;

	typedef struct Ymsg_s
	{
		Ylog_level level;
		char msg[MAX_LOG_LINE];
	}Ymsg;

	typedef struct Ylogger_s Ylogger;
	typedef struct Ylogger_options_s
	{
		int level;
		char *path;
		int max_size_bytes;
		char *format;
	}Ylogger_options;

	/*
	 * 描述：
	 * Ylog全局初始化函数，该函数在整个应用程序里只调用一次
	 *
	 * 返回值：
	 * YERRNO
	 */
	YAPI int Y_log_init(const char *config);

	YAPI Ylogger *Y_log_get_logger(const char *name);

	YAPI void Y_log_write(Ylogger *logger, Ylog_level level, int line, char *msg, ...);
#endif


#ifndef YFILE
#define YFILE

	// 存储文件信息
	typedef struct Yfstat_s
	{
		int exist;
		uint64_t length;
	}Yfstat;

	/*
	 * 描述：
	 * 以字节为单位返回文件大小
	 *
	 * 参数：
	 * @file_path：要读取的文件的完整路径
	 *
	 * 返回值：
	 * 文件大小，以字节为单位
	 */
	YAPI int Y_file_stat(const char *file_path, Yfstat *stat);


	/*
	 * 描述：
	 * 读取一个文件里的所有字节
	 *
	 * 参数：
	 * @file_path：要读取的文件的完整路径
	 * @bytes：字节缓冲区
	 *
	 * 返回值：
	 * 文件内容的长度
	 */
	YAPI int Y_file_readbytes(const char *file_path, char **bytes, uint64_t *size);

	/*
	 * 描述：
	 * 释放使用Y_file_readall函数开辟的内存空间
	 *
	 * 参数：
	 * @content：要释放的内存空间
	 */
	YAPI void Y_file_free(char *content);

#endif


/***********************************************************************************
 * @ file    : Ylits.h
 * @ author  : oheiheiheiheihei
 * @ version : 0.9
 * @ date    : 2021.12.06
 * @ brief   : 实现一个动态数组
 ************************************************************************************/
#ifndef YLIST
#define YLIST
    
    typedef struct Ylist_s Ylist;

    typedef void (*Ylist_free_func)(void *item);

	typedef int (*Ylist_foreach_func)(Ylist *yl, void *item, void *userdata);

    typedef int (*Ylist_query_func)(Ylist *yl, void *item, void *data, void *userdata);

    YAPI Ylist *Y_create_list();

    YAPI Ylist *Y_create_list2(Ylist_free_func freefunc);

    /*
     * 描述：
     * 释放一个集合
     * 如果集合里当前有元素并且你设置了Y_lits_free_func，那么这个函数会帮你调用freefunc并释放集合对象
     * 
     * 参数：
     * @yl：要操作的集合
     */
    YAPI void Y_delete_list(Ylist *yl);

    /*
     * 描述：
     * 对集合做foreach遍历操作
     * 
     * 参数：
     * @yl：要操作的集合
	 * @ff：遍历回调函数，通过返回值来指定是否继续遍历
     * 
     * 返回：
     * 最后一次遍历的返回值
     */
    YAPI int Y_list_foreach(Ylist *yl, Ylist_foreach_func ff, void *userdata);

    /*
     * 描述：
     * 往集合里插入一个元素
     * 如果集合的当前空间不够存储新的元素，那么会自动扩充空间，新扩充的空间的大小是当前大小的两倍
     * 
     * 参数：
     * @yl：要操作的集合
	 * @item：要插入的元素
     */
    YAPI void Y_list_add(Ylist *yl, void *item);

    /*
     * 描述：
     * 清空集合里的元素
     * 如果你设置了Ylist_free_func，那么这个函数会帮你调用freefunc，然后把length设置成0
     * 
     * 参数：
     * @yl：要操作的集合
     */
    YAPI void Y_list_clear(Ylist *yl);

    /*
     * 描述：
     * 获取集合里元素的数量
     * 
     * 参数：
     * @yl：要操作的集合
     */
    YAPI int Y_list_count(Ylist *yl);

    /*
     * 描述：
     * 判断一个元素是否存在于集合里
     * 
     * 参数：
     * @yl：要判断的集合
     * @item：要判断的元素
     * 
     * 返回值：
     * 存在返回元素的索引，不存在返回-1
     */
    YAPI int Y_list_contains(Ylist *yl, void *item);

    /*
     * 描述：
     * 向集合里插入一个元素
     * 
     * 参数：
     * @yl：要插入的集合
     * @index：要插入的索引位置
     * @item：要插入的元素
     * 
     * 返回值：
     * 存在返回元素的索引，不存在返回-1
     */
    YAPI void Y_list_insert(Ylist *yl, int index, void *item);

    /*
     * 描述：
     * 从集合里移除一个元素
     * 
     * 参数：
     * @yl：要移除元素的集合
     * @item：要移除的元素
     * @free：如果free == 1，并且你指定了freefunc，那么该函数会帮你free掉item
     */
    YAPI void Y_list_remove(Ylist *yl, void *item, int free);

    /*
     * 描述：
     * 根据索引从集合里移除一个元素
     * 
     * 参数：
     * @yl：要移除元素的集合
     * @at：要移除的元素的索引
     * @free：如果free == 1，并且你指定了freefunc，那么该函数会帮你free掉item
     */
    YAPI void Y_list_removeat(Ylist *yl, int at, int free);

    /*
     * 描述：
     * 根据条件查询一个元素
     *
     * 参数：
     * @yl：要移除元素的集合
     */
    YAPI void *Y_list_query(Ylist *yl, Ylist_query_func queryfunc, void *data, void *userdata);
#endif

/***************************************************************************************************************************
 * @ file    : Ymap.h
 * @ author  : oheiheiheiheihei
 * @ version : 0.9
 * @ date    : 2021.12.09 09:46
 * @ brief   : 实现一个哈希表
 * @ ref     ：https://zhuanlan.zhihu.com/p/95156642
 * @ remark  ：开启Y_MAP_CHAIN宏，指定使用链式扩展法解决hash冲突，使用除留余数计算hash值
 * 散列表：把数据分散着排列，分开排列
 **************************************************************************************************************************/
#ifndef YMAP
#define YMAP

    // Ymap对象
    typedef struct Ymap_s Ymap;

    // 计算hash值的函数
    typedef int(*Ymap_hash_func)(void *key);

    // 比较key的函数
    // 如果key相等，那么返回1
    // 如果key不相等，那么返回0
    typedef int(*Ymap_keycmp_func)(void *key1, void *key2);

    // 用来释放value的函数
    typedef void(*Ymap_free_func)(void *value);

    // 遍历哈希表的函数
    typedef void(*Ymap_foreach_func)(Ymap *ym, void *key, void *value, void *userdata);


    // 预定义的哈希函数
    // 使用java里的string类型的hash函数
    YAPI int Y_map_hash_string_java(void *key);

    // 预定义的字符串键比较函数
    YAPI int Y_map_keycmp_string(void *key1, void *key2);

    // 预定义的指针类型的键比较函数
    YAPI int Y_map_keycmp_pointer(void *key1, void *key2);


	/*
	 * 描述：
	 * 创建一个哈希表对象
	 *
	 * 返回值：
	 * 新创建的哈希表对象
	 */
    YAPI Ymap *Y_create_map(Ymap_hash_func hash, Ymap_keycmp_func keycmp, Ymap_free_func free);

	/*
	 * 描述：
	 * 删除hash表对象
	 *
     * 参数：
     * @ym：要删除的哈希表对象
	 */
    YAPI void Y_delete_map(Ymap *ym);

    /*
     * 描述：
     * 查找对应key的value
     *
     * 参数：
     * @ym：要操作的哈希表对象
     * @key：要查找的键
     *
     * 返回值：
     * key对应的value，注意value可能是NULL
     */
    YAPI void *Y_map_get(Ymap *ym, void *key);

	/*
	 * 描述：
	 * 往hash表里存储一个键值对
	 * 
     * 参数：
     * @ym：要操作的哈希表对象
     * @key：要存储的键
     * @value：要存储的值
     * 
     * 返回值：
     * 如果该key已经存在，那么返回该key所对应的value
     * 如果该key不存在，那么返回value
	 */
    YAPI void *Y_map_set(Ymap *ym, void *key, void *value);

	/*
	 * 描述：
	 * 根据key移除一个键值对
	 *
     * 参数：
     * @ym：要删除的哈希表对象
     * @key：要删除的键值对的键
     * 
     * 注意，如果你设置了Ymap_free_func，那么该函数会帮你释放掉value
	 */
    YAPI void Y_map_remove(Ymap *ym, void *key);

	/*
	 * 描述：
	 * 清空hash表
	 *
     * 参数：
     * @ym：要清空的表
     * 
     * 注意，如果你设置了Ymap_free_func，那么该函数会帮你释放掉所有的value
	 */
    YAPI void Y_map_clear(Ymap *ym);

	/*
	 * 描述：
	 * 获取hash表里的元素的个数
	 *
     * 参数：
     * @ym：要获取元素个数的hash表
     * 
     * 返回值：
     * hash表里元素的个数
	 */
    YAPI int Y_map_count(Ymap *ym);

    /*
     * 描述：
     * 对hash表进行遍历操作
     *
     * 参数：
     * @ym：要遍历的表
     * @foreach：遍历函数
     * @userdata：遍历过程中用户自定义数据
     *
     * 注意，哈希表的遍历有可能是无序的
     */
    YAPI void Y_map_foreach(Ymap *ym, Ymap_foreach_func foreach, void *userdata);
#endif

/*****************************************************************************************************************
 * @ file    : Ypool.h
 * @ author  : oheiheiheiheihei
 * @ version : 0.9
 * @ date    : 2021.12.06 16:43
 * @ brief   : 
 *****************************************************************************************************************/
#ifndef YPOOL
#define YPOOL

	typedef struct Yobject_s Yobject;
	typedef struct Ypool_s Ypool;

	/*
	 * 描述：
	 * 创建一个对象缓冲池
	 *
	 * 参数：
	 * @max_block_size：缓冲池里可以申请的最大的缓冲区大小
	 * @max_blocks：缓冲池里的内存块的最大个数
	 *
	 * 返回值：
	 * 创建的缓冲池对象
	 */
	YAPI Ypool *Y_create_pool(int max_block_size, int max_blocks);

	/*
	 * 描述：
	 * 从对象缓冲池里获取一个对象
	 * 如果没有多余的对象了，那么会重新创建一个
	 * 如果缓冲池里的对象数量大于最大数量，那么会直接创建一个新的对象
	 *
	 * 参数：
	 * @yp：缓冲池对象
	 * @blocksize：要申请的内存块的大小
	 *
	 * 返回值：
	 * 缓冲对象
	 */
	YAPI Yobject *Y_pool_obtain(Ypool *yp, int blocksize);

	/*
	 * 描述：
	 * 回收缓冲对象，以便于下次继续使用
	 * 当你从缓冲池里obtain的对象用完了的时候请调用这个函数
	 * 这个函数会把你用完的对象重新放到缓冲池里，以便于下次复用
	 *
	 * 参数：
	 * @yo：要回收的对象
	 */
	YAPI void Y_pool_recycle(Yobject *yo);

	/*
	 * 描述：
	 * 获取Yobject里你开辟的内存空间
	 *
	 * 返回值：
	 * 内存地址
	 */
	YAPI void *Y_object_get_data(Yobject *yo);
#endif


/***********************************************************************************
 * @ file    : Yqueue.h
 * @ author  : oheiheiheiheihei
 * @ version : 0.9
 * @ date    : 2022.09.24
 * @ brief   : 实现一个先进先出的队列
 ************************************************************************************/
#ifndef YQUEUE
#define YQUEUE

    typedef struct Yqueue_s Yqueue;

    YAPI Yqueue *Y_create_queue();

    /*
     * 描述：
     * 释放一个集合
     * 如果集合里当前有元素并且你设置了Y_lits_free_func，那么这个函数会帮你调用freefunc并释放集合对象
     * 
     * 参数：
     * @yl：要操作的集合
     */
    YAPI void Y_delete_queue(Yqueue *yq);

    /*
     * 描述：
     * 往集合里插入一个元素
     * 如果集合的当前空间不够存储新的元素，那么会自动扩充空间，新扩充的空间的大小是当前大小的两倍
     * 
     * 参数：
     * @yl：要操作的集合
	 * @item：要插入的元素
     */
    YAPI void Y_queue_enqueue(Yqueue *yq, void *item);

    /*
     * 描述：
     * 清空集合里的元素
     * 如果你设置了Ylist_free_func，那么这个函数会帮你调用freefunc，然后把length设置成0
     * 
     * 参数：
     * @yl：要操作的集合
     */
    YAPI void *Y_queue_dequeue(Yqueue *yl);

    /*
     * 描述：
     * 获取集合里元素的数量
     * 
     * 参数：
     * @yl：要操作的集合
     */
    YAPI int Y_queue_size(Yqueue *yq);

    YAPI void Y_queue_clear(Yqueue *yq);
#endif

#ifndef YSTRING
#define YSTRING

    typedef enum Ystr_split_opts_e
	{
		SSO_None = 0,
		SSO_RemoveEmptyEntries = 1
	}Ystr_split_opts;

	YAPI void Ystr_trim_right(char *str, char c);

	/*
	 * 描述：
	 * 生成一个随机数
	 *
	 * 参数：
	 * @buffer：存储随机数的缓冲区
	 * @length：缓冲区大小
	 */
	YAPI void Ystr_rand(char *buffer, size_t length);

	/*
	 * 描述：
	 * 把str按照separator进行分割
	 *
	 * 参数：
	 * @str：要分割的字符串
	 * @separator：分隔符
	 * @opt：分割选项
	 * @tokens：存储分割后的字符串的数组
	 * @total_tokens：字符串数组的长度
	 *
	 * 返回：
	 * 分割出的字符串的个数
	 */
	YAPI int Ystr_split(const char *str, const char separator, Ystr_split_opts opt, char **tokens, size_t total_tokens);

	/*
	 * 描述：
	 * 打印分割后的字符串（调试用）
	 *
	 * 参数：
	 * @num：字符串数组的长度
	 * @tokens：分割后的字符串数据
	 */
	YAPI void Ystr_split_print(char **tokens, int num);

	/*
	 * 描述：
	 * 释放分割后的字符串内存
	 *
	 * 参数：
	 * @num_tokens：字符串数组的长度
	 * @tokens：要释放的字符串数组
	 */
	YAPI void Ystr_split_free(char **tokens, size_t num_tokens);
#endif

/***********************************************************************************
 * @ file    : Ytree.h
 * @ author  : oheiheiheiheihei
 * @ version : 0.9
 * @ date    : 2021.12.06
 * @ brief   : 一颗多叉树
 ************************************************************************************/
#ifndef YTREE
#define YTREE

    #define MAX_CHILD_COUNT 1024

    typedef struct Ytree_s Ytree;
    typedef struct Ytreenode_s Ytreenode;

    struct Ytreenode_s
    {
        // 该节点保存的数据
        void *data;

        // 子节点列表
        Ytreenode *children[MAX_CHILD_COUNT];

        // 子节点数量
        int num_child;

        // 父节点
        Ytreenode *parent;
    };

    typedef void (*Ytree_free_func)(void *data);

	typedef int (*Ytree_foreach_action)(Ytree *tree, Ytreenode *node, void *userdata);

    /*
     * 描述：
     * 实例化一棵树
     */
    YAPI Ytree *Y_create_tree();

    YAPI Ytree *Y_create_tree2(Ytree_free_func freefunc);

    /*
     * 描述：
     * 删除并释放一棵树所占用的所有资源
     * 
     * 参数：
     * @tree：要删除的tree对象
     */
    YAPI void Y_delete_tree(Ytree *tree);

    YAPI Ytreenode *Y_tree_initroot(Ytree *tree, void *data);

    /*
     * 描述：
     * 新建一个节点
     * 
     * 参数：
     * @tree：tree对象
     * @foreach_action：遍历函数
     * @userdata：用户自定义数据
     */
    YAPI Ytreenode *Y_tree_newnode(Ytreenode *parent, void *data);

    /*
     * 描述：
     * 判断该树是否是一颗空树
     */
    YAPI int Y_tree_isempty(Ytree *tree);

    /*
     * 描述：
     * 对tree进行遍历操作，使用深度优先遍历
     * 
     * 参数：
     * @tree：tree对象
     * @foreach_action：遍历函数
     * @userdata：用户自定义数据
     */
    YAPI void Y_tree_foreach(Ytree *tree, Ytree_foreach_action foreach_action, void *userdata);

    /*
     * 描述：
     * 对tree进行遍历操作，使用广度优先遍历
     * 
     * 参数：
     * @tree：tree对象
     * @foreach_action：遍历函数
     * @userdata：用户自定义数据
     */
    YAPI void Y_tree_foreach2(Ytree *tree, Ytree_foreach_action foreach_action, void *userdata);

    /*
     * 描述：
     * 删除某个节点以及子节点
     * 
     * 参数：
     * @tree：tree对象
     * @node：要删除的节点
     */
    YAPI void Y_tree_delete(Ytree *tree, Ytreenode *node);

    YAPI void Y_tree_clear(Ytree *tree);
#endif

/***********************************************************************************
 * @ file    : Ybuffer_queue.h
 * @ author  : oheiheiheiheihei
 * @ version : 0.9
 * @ date    : 2021.12.06
 * @ brief   : 使用环形缓冲区实现一个生产者 - 消费者模型的队列
 * @ remark  ：如果入队的速度比出队的速度快，那么有可能会出现还没出队的元素被新的数据覆盖掉的情况
 ************************************************************************************/
#ifndef YBUFFER_QUEUE
#define YBUFFER_QUEUE
    
	typedef enum
	{
		YQUEUE_STATE_IDLE,
		YQUEUE_STATE_RUNNING
	}Yqueue_state;

	typedef void(*Yqueue_callback)(void *userdata, void *element);
	typedef void(*Yqueue_full_callback)(void *element, void *userdata);
	typedef struct Ybuffer_queue_s Ybuffer_queue;

	/*
	 * 描述：
	 * 创建一个缓冲队列
	 * 该队列使用单独的线程消费元素，并提供消费者callback给调用者使用
	 * 入队和出队是在不同线程进行，互不影响
	 * 队列有着固定的大小，类似于环形缓冲区
	 * 如果队列满了而此时还在继续入队，那么丢弃最早入队的元素，并把入队的元素从头开始插入
	 *
	 * 参数：
	 * @userdata：用户自定义对象，在回调里会当成参数回调给用户
	 *
	 * 返回值：
	 * Yqueue对象
	 *
	 * 不要直接调用Y_queue_enqueue和Y_queue_dequeue函数，Y_queue_start会自动调用该函数并通过回调的方式把element返回
	 * 除非你没有调用Y_queue_start，那么请永远不要直接调用这两个函数
	 */
	YAPI Ybuffer_queue *Y_create_buffer_queue(void *userdata);

	/*
	 * 描述：
	 * 删除队列并释放所占用的资源
	 *
	 * 参数：
	 * @q：要删除的队列
	 */
	YAPI void Y_delete_buffer_queue(Ybuffer_queue *yq);

	/*
	 * 描述：
	 * 创建一个线程并开始消费元素
	 *
	 * 参数：
	 * @q：要操作的队列对象
	 * @num_thread：消费者线程的数量
	 * @callback：每消费了一个元素，就会通过该回调回调给用户，用户可以在该回调里做操作
	 */
	YAPI void Y_buffer_queue_start(Ybuffer_queue *yq, int num_thread, Yqueue_callback callback);

	/*
	 * 描述：
	 * 设置当缓冲队列满了之后，溢出的元素如何处理的回调
	 *
	 * 参数：
	 * @yq：要设置的队列对象
	 * @callback：当缓冲区溢出的时候，回调溢出的元素
	 */
	YAPI void Y_buffer_queue_set_full_callback(Ybuffer_queue *yq, Yqueue_full_callback callback);

	/*
	 * 描述：
	 * 出队函数
	 *
	 * 参数：
	 * @q：要出队的对象
	 *
	 * 返回值：
	 * 出队的对象
	 */
	YAPI void *Y_buffer_queue_dequeue(Ybuffer_queue *yq);

	/*
	 * 描述：
	 * 入队函数
	 *
	 * 参数：
	 * @q：要入队的对象
	 * 
	 * 返回值：
	 * 0表示成功，否则参考错误码
	 */
	YAPI void Y_buffer_queue_enqueue(Ybuffer_queue *yq, void *element);

	/*
	 * 描述：
	 * 获取当前队列的大小
	 */
	YAPI int Y_buffer_queue_size(Ybuffer_queue *yq);


	/*
	 * 描述：
	 * 清空缓冲队列里的元素
	 */
	YAPI void Y_buffer_queue_clear(Ybuffer_queue *yq);
#endif


/***********************************************************************************
 * @ file    : Ythread.h
 * @ author  : oheiheiheiheihei
 * @ version : 0.9
 * @ date    : 2021.12.09 19:28
 * @ brief   : 封装不同平台下的线程函数
 ************************************************************************************/
#ifndef __YTHREAD_H__
#define __YTHREAD_H__

    typedef struct Ythread_s Ythread;

    typedef void(*Ythread_entry)(void *userdata);

    /*
     * 描述：
     * 创建一个Ythread对象并开始运行
     *
     * 参数：
     * @entry：线程函数
     * @userdata：传递给线程的自定义数据
     * 
     * 返回值：
     * Ythread对象
     */
    YAPI Ythread *Y_create_thread(Ythread_entry entry, void *userdata);

    /*
     * 描述：
     * 删除一个Ythread对象
     * 该函数会先等待线程运行完毕，然后再释放掉Ythread
     *
     * 参数：
     * @yt：要释放的thread对象
     */
    YAPI void Y_delete_thread(Ythread *yt);

#endif


#endif

