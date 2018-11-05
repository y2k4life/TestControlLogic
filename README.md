# Control Logic and Patter Matching

---

**UPDATE 11-04-2018**

I made a major blunder on the test which lead me to believe `enum` and Pattern Matching are equal. **They are not!!!** I ran the same test for both. I fixed that bug. I also added Randomizing test seggested by a reader. My first thought was that I wanted to make sure all test are equal and to me randomizing woudl not guarantee that, but there is this thing [Branch Prediction](https://stackoverflow.com/a/11227902/3764814) which optimizes branching code which defeats what I was trying to do. By repeating the same jump over and over the CPU optimized that and creates a shortcut.

---
## Summary

Following along with [@terrajobst](https://github.com/terrajobst) as he builds a compiler [Minsk](https://github.com/terrajobst/minsk) I suggested using [Pattern Matching](https://docs.microsoft.com/en-us/dotnet/csharp/pattern-matching) for a block of code. The discussion in my PR was around efficiency and the difference of using an `enum` with a `switch` statement instead of using Pattern Matching. The theory is that an enum/int would only jump once and cast only once. The theory is that pattern matching would do multiple tests and jumps and would have to cast for each `case` statement. I started to ponder why have Pattern Matching if it was inefficient and doing multiple casting when testing against a type. One alternative to Pattern Matching is to add a `Kind` property to an object of an `enum` type then do a cast in the `case` statement.

Turns out with Pattern Matching casting is only done once and I will explain that below. It does turn out though that Pattern Matching compiled to IL code has some inefficiencies.

## Analyzing the Code

I decided to write some code (copied from Minsk), look at IL, and do some benchmarks. My goal was to compare and analyze the following.

```c#
//
// Switch_Pattern_Matching
//
switch (node)
{
    case BoundLiteralExpression literalExpression:
        return EvaluateLiteralExpression(literalExpression);
    //
    //   More case statements  
    //
    default:
        return null:
}
```

vs

```c#
//
// Switch_Enum_Casting
//
switch (node.Kind)
{
    case BoundNodeKind.LiteralExpression:
        return EvaluateLiteralExpression((BoundLiteralExpression)node);
    //
    //   More case statements  
    //
    default:
        return null;
}
```

I also wanted to compare those with an `if else` statement using `enum` vs Pattern Matching

```C#
//
// If_Pattern_Matching
//
if (node is BoundLiteralExpression literalExpression)
{
    return EvaluateLiteralExpression(literalExpression);
}
//
// Multiple if else
//
return null
```

vs

```c#
//
// If_Enum_Casting
//

if (node.Kind == BoundNodeKind.LiteralExpression)
{
    return EvaluateLiteralExpression((BoundLiteralExpression)node);
}
//
// Multiple if else
//
return null
```

## Benchmarks

The benchmark results below are from calling the methods above with forcing the last test statement being the one to satisfy. Forcing the worse case scenario of having to test all possibilities. The benchmarks called the methods 100,000 times and then 500,000 times. **Update** this turns out not to be true, CPU does optimization and will short circuit the intended results. Randomization was added. I wanted the test to be identical which I can't guarantee with randomization, but it is a trade off to avoid CPU optimization.

Pattern matching is faster, but that does not me the IL code is efficient. I will also have more updates with there are more possibilities beyond the 5 we have now.

``` ini

BenchmarkDotNet=v0.11.2, OS=Windows 10.0.17134.345 (1803/April2018Update/Redstone4)
Intel Core i7-7820HK CPU 2.90GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=2.1.500-preview-009335
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT  [AttachedDebugger]
  DefaultJob : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT


```
|                         Method |      N |      Mean |     Error |    StdDev |
|------------------------------- |------- |----------:|----------:|----------:|
|        **Switch_Pattern_Matching** | **100000** |  **1.197 ms** | **0.0227 ms** | **0.0270 ms** |
|            Switch_Enum_Casting | 100000 |  1.538 ms | 0.0243 ms | 0.0227 ms |
|            If_Pattern_Matching | 100000 |  1.157 ms | 0.0230 ms | 0.0264 ms |
|                If_Enum_Casting | 100000 |  1.862 ms | 0.0170 ms | 0.0159 ms |
| **Random_Switch_Pattern_Matching** | **100000** | **14.768 ms** | **0.3329 ms** | **0.3963 ms** |
|     Random_Switch_Enum_Casting | 100000 | 15.258 ms | 0.3213 ms | 0.4709 ms |
|     Random_If_Pattern_Matching | 100000 | 14.579 ms | 0.2749 ms | 0.2572 ms |
|         Random_If_Enum_Casting | 100000 | 15.322 ms | 0.0943 ms | 0.0787 ms |
|        **Switch_Pattern_Matching** | **500000** |  **5.778 ms** | **0.0564 ms** | **0.0527 ms** |
|            Switch_Enum_Casting | 500000 |  7.697 ms | 0.0803 ms | 0.0671 ms |
|            If_Pattern_Matching | 500000 |  5.627 ms | 0.0251 ms | 0.0222 ms |
|                If_Enum_Casting | 500000 |  9.297 ms | 0.0438 ms | 0.0388 ms |
| **Random_Switch_Pattern_Matching** | **500000** | **71.938 ms** | **0.7353 ms** | **0.6878 ms** |
|     Random_Switch_Enum_Casting | 500000 | 75.282 ms | 0.9671 ms | 0.9046 ms |
|     Random_If_Pattern_Matching | 500000 | 72.064 ms | 0.4969 ms | 0.4405 ms |
|         Random_If_Enum_Casting | 500000 | 76.534 ms | 0.7372 ms | 0.6536 ms |


## Analyzing IL code

I looked at the IL code generated for Pattern Matching to see if casting was done multiple times. Looking at the benchmarks I doubted that. If casting was done each time the logic as I reasoned would be a `try catch`. Try to cast `foo` to `bar` if that is successful then return `bar` if an error is thrown then catch and return `null`. I wrote code to do the `try catch` benchmarked it at 100 times and took about 19ms.

```IL
  IL_0000:  ldarg.1                  // I think this will take arg1 - node
  IL_0001:  stloc.0                  // store as var 0
.
.
.
  IL_0023:  ldloc.0                  // load var 0 (node) on to the stack
  IL_0024:  isinst     Minsk.CodeAnalysis.Binding.BoundUnaryExpression          // test type if match then cast and push onto stack otherwise push null onto stack
  IL_0029:  dup
  IL_002a:  stloc.s    V_4           // store the value on the stack to V_4
  IL_002c:  brtrue.s   IL_005f       // branch if matched
  IL_002e:  ldloc.0                  // load var 0 (node) on to the stack
  IL_002f:  isinst     Minsk.CodeAnalysis.Binding.BoundBinaryExpression         // test type if match then cast and push onto stack otherwise push null onto stack
  IL_0034:  dup
  IL_0035:  stloc.s    V_5           // store the value on the stack to V_5
  IL_0037:  brfalse.s  IL_0079       // branch if they don't match
  IL_0039:  br.s       IL_006c       // branch if they do match
```

The interest piece of this code is the `isinist`. Referring to ECMA-335

> isinst typeTok
>
> Test if obj is an instance of typeTok, returning null or an instance of
that class or interface.
>
> The isinst instruction tests whether obj (type O) is an instance of the type typeTok.
If the actual type (not the verifier tracked type) of obj is verifier-assignable-to the type
typeTok then isinst succeeds and obj (as result) is returned unchanged while verification
tracks its type as typeTok. Unlike coercions (§III.1.6) and conversions (§III.3.27), isinst
never changes the actual type of an object and preserves object identity (see Partition I).
If obj is null, or obj is not verifier-assignable-to the type typeTok, isinst fails and returns null.

## Efficiency

Casting is not an issue based on the ECMA-336 documentation because casting is not done unless there is a match. Upon further review of the IL code what I noticed as being inefficient is Pattern Matching declares a variable for all possibilities. As the Minsk compiler grows there could be up to 21 possibilities. That could be up to 42 variables. Where as with `switch enum` there is only one, the `enum` value.

```IL
//
// Switch_Pattern_Matching
//
  .locals init (class Minsk.CodeAnalysis.Binding.BoundExpression V_0,
           class Minsk.CodeAnalysis.Binding.BoundLiteralExpression V_1,
           class Minsk.CodeAnalysis.Binding.BoundVariableExpression V_2,
           class Minsk.CodeAnalysis.Binding.BoundAssignmentExpression V_3,
           class Minsk.CodeAnalysis.Binding.BoundUnaryExpression V_4,
           class Minsk.CodeAnalysis.Binding.BoundBinaryExpression V_5,
           class Minsk.CodeAnalysis.Binding.BoundLiteralExpression V_6,
           class Minsk.CodeAnalysis.Binding.BoundVariableExpression V_7,
           class Minsk.CodeAnalysis.Binding.BoundAssignmentExpression V_8,
           class Minsk.CodeAnalysis.Binding.BoundUnaryExpression V_9,
           class Minsk.CodeAnalysis.Binding.BoundBinaryExpression V_10)
```

vs

```IL
//
// Switch_Enum_Casting
//
.locals init (valuetype Minsk.CodeAnalysis.Binding.BoundNodeKind V_0)
```

Another part of compiled `switch enum` IL code that is more efficient is with jump logic, because `switch enum` uses a jump table with the `int` value of the `enum` being an index into that jump table. There is one entry for every `case` statement. Instead of the 5 lines of code to push/pop and compare when using Pattern Matching. Minsk could have 21 possibilities which is 105 lines of IL code for Pattern Matching compared to 1 line with a 21 entries when using `switch enum`.

```IL
  IL_0008:  switch     (
                        IL_0023,
                        IL_0030,
                        IL_003d,
                        IL_004a,
                        IL_0057)
```

vs

```il
  IL_0002:  ldloc.0
  IL_0003:  brfalse.s  IL_0079
  IL_0005:  ldloc.0
  IL_0006:  isinst     Minsk.CodeAnalysis.Binding.BoundLiteralExpression
  IL_000b:  dup
  IL_000c:  stloc.1
  IL_000d:  brtrue.s   IL_003b
  IL_000f:  ldloc.0
  IL_0010:  isinst     Minsk.CodeAnalysis.Binding.BoundVariableExpression
  IL_0015:  dup
  IL_0016:  stloc.2
  IL_0017:  brtrue.s   IL_0047
  IL_0019:  ldloc.0
  IL_001a:  isinst     Minsk.CodeAnalysis.Binding.BoundAssignmentExpression
  IL_001f:  dup
  IL_0020:  stloc.3
  IL_0021:  brtrue.s   IL_0053
  IL_0023:  ldloc.0
  IL_0024:  isinst     Minsk.CodeAnalysis.Binding.BoundUnaryExpression
  IL_0029:  dup
  IL_002a:  stloc.s    V_4
  IL_002c:  brtrue.s   IL_005f
  IL_002e:  ldloc.0
  IL_002f:  isinst     Minsk.CodeAnalysis.Binding.BoundBinaryExpression
  IL_0034:  dup
  IL_0035:  stloc.s    V_5
  IL_0037:  brfalse.s  IL_0079
  IL_0039:  br.s       IL_006c
```

>switch ( t1, t2 … tN )
>
>The switch instruction implements a jump table. The format of the instruction is an unsigned
int32 representing the number of targets N, followed by N int32 values specifying jump targets:
these targets are represented as offsets (positive or negative) from the beginning of the
instruction following this switch instruction.
>
>The switch instruction pops value off the stack and compares it, as an unsigned integer, to n. If
value is less than n, execution is transferred to the value’th target, where targets are numbered
from 0 (i.e., a value of 0 takes the first target, a value of 1 takes the second target, and so on). If
value is not less than n, execution continues at the next instruction (fall through).

## Conclusion

> Efficiency does not always mean faster.

> Watch out for syntactic sugar.

Keep in mind this is all theoretical, who cares, in most cases only having 1 to 5 different types would not make a difference. But when you have 20 or even 100 or more and calling a method multiple times a second something needs to change. One would ask why all of this fuss for a mere millisecond of time or who cares about having multiple variables declared when you have 16Gb or more of memory. I'm from the days of having to write the OS and the app with maybe 32k of memory and a CPU running at 1Mhz.

## References

[Pattern Matching](https://docs.microsoft.com/en-us/dotnet/csharp/pattern-matching)

[is keyword](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/is)

[as keyword](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/as)

[ECMA-335](http://www.ecma-international.org/publications/standards/Ecma-335.htm)

[List of CIL instructions](https://en.wikipedia.org/wiki/List_of_CIL_instructions)

[Ildasm.exe (IL Disassembler)](https://docs.microsoft.com/en-us/dotnet/framework/tools/ildasm-exe-il-disassembler)