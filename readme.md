# MoonSounds

Moonsounds allows you to define a waveform as a lua function and convert it into a wav file.

## How to use it

Create a file with a .lua extension, for example: `test.lua`

This file must end with a `return` statement that returns a function that takes one parameter.

That function returns the `y` value of the waveform for the given time `t` in seconds.

For example:

```lua
-- contents of test.lua
return function(t)
    -- create a function that represents a normalized (1hz) sine wave
    function normalSin(pitch)
        return math.sin(t * math.pi * 2 * pitch)
    end

    -- add together 261.6hz, 329.6hz, and 392.0hz, then divide the result by 3
    return (normalSin(261.6) + normalSin(329.6) + normalSin(392.0)) / 3
end
```

Run Moonsounds passing in that file. The easiest way to do that is:

```sh
dotnet run --project MoonSounds test.lua
```

> Note, you need to have dotnet 8.0 or greater installed for this to work.

After a few seconds this will generate a file `test.wav` that you can play with your audio player of choice.