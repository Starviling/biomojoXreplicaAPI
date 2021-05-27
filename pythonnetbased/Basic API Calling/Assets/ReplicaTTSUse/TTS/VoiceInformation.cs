using System;
using System.Collections.Generic;

class VoiceClip
{
    string uuid;
    string generation_time;
    string url;
    string duration;
    string speaker_id;
    string txt;
    string bit_rate;
    string sample_rate;
    string extension;

    public string Uuid { get => uuid; set => uuid = value; }
    public string Generation_time { get => generation_time; set => generation_time = value; }
    public string Url { get => url; set => url = value; }
    public string Duration { get => duration; set => duration = value; }
    public string Speaker_id { get => speaker_id; set => speaker_id = value; }
    public string Txt { get => txt; set => txt = value; }
    public string Bit_rate { get => bit_rate; set => bit_rate = value; }
    public string Sample_rate { get => sample_rate; set => sample_rate = value; }
    public string Extension { get => extension; set => extension = value; }
}

class AvailableVoices
{

}