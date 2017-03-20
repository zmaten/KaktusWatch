open System
open Worker


// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

type FacebookPost = { 
    Message: string 
    CreatedTime: DateTime
    }

 type FacebookFeed = {
    Data: List<FacebookPost>
    }

let kaktusFBUrl = "https://graph.facebook.com/Kaktus/posts?access_token=1672094689755085|671e0538eaaffd57d780c950b713584c"
let triggerTime = 60

let IsPromotion (post:FacebookPost) (triggerInterval:int) =
    post.CreatedTime >= DateTime.UtcNow - TimeSpan(0, triggerInterval, 0) &&
    post.Message.Contains("mujkaktus.cz/chces-pridat")
    

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    let jsonData = fetchUrl myCallback kaktusFBUrl
    printf "%A" jsonData
    0 // return an integer exit code