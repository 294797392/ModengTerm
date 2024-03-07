#ifndef __VTYPEFACE_H__
#define __VTYPEFACE_H__

#ifdef __cplusplus 
extern "C" {
#endif

    typedef struct VTypeface
    {
        double height;
        double spaceWidth;
        char fontFamily[32];
        double fontSize;
        char backgroundColor[32];
        char foregroundColor[32];
    }VTypeface;

#ifdef __cplusplus
}
#endif

#endif