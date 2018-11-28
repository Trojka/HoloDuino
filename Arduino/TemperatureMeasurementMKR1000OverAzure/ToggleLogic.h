#ifndef TOGGLELOGIC_H
#define TOGGLELOGIC_H

#ifdef __cplusplus
extern "C" {
#endif

int actionPin = 6;
int actionState = LOW;
int resetPin = 5;

void DoToggleLogic();

#ifdef __cplusplus
}
#endif

#endif /* TOGGLELOGIC_H */
