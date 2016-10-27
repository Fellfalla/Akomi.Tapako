powershell -Command "& {(dir -include *.xaml, *.cs,*.cpp,*.h,*.idl,*.asmx -recurse | select-string .).Count}"

PAUSE