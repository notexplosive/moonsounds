-- t is time in seconds
return function(t)
    function SIN(pitch)
        return math.sin(t * math.pi * 2 * pitch)
    end

    return (SIN(261.6) + SIN(329.6) + SIN(392.0)) / 3
end