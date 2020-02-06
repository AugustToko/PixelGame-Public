var speed : float = 30.0;
 private var pivot : Vector3 = new Vector3(0,50,0);
 
 function Update () {
     transform.RotateAround(pivot, Vector3.up, speed * Time.deltaTime);
 }