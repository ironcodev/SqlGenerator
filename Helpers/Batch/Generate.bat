@echo off
set GenerationType=DropCreate
set SubType=All

if [%2]==[] (set GenerationType=DropCreate) else (set GenerationType=%2)
if [%3]==[] (set SubType=All) else (set SubType=%3)

sqlgen -o -gt %GenerationType% -t %1 -st %SubType%
