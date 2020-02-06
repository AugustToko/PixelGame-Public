// Pixel Weather Scaler
// Asset Publisher: Grass Depot
// Copyright 2015 David Schmeltekopf

class PixelWeatherScaler extends EditorWindow {
	
	var scaleMultiplier : float = 2.0f;
	@MenuItem("Window/Pixel Weather Scale Tool")
    static function ShowWindow() {
        var win = EditorWindow.GetWindow(PixelWeatherScaler);
        #if UNITY_5_0
    	win.title = "Pixel Weather";
		#else
    	win.titleContent = GUIContent("Pixel Weather");
		#endif
        
        win.minSize = new Vector2(340, 110);
    }

    function OnGUI() {
        EditorGUILayout.LabelField("Pixel Weather Scale Tool", EditorStyles.boldLabel);
        
        var selectedParticleString : String = "Select a particle or weather system";
        if (Selection.gameObjects.length == 1)
            selectedParticleString = "Selected Particle : " + Selection.gameObjects[0].name;
        else if (Selection.gameObjects.length > 1)
            selectedParticleString = "Selected Particle : " + Selection.gameObjects.length + " particles selected";
        EditorGUILayout.LabelField(selectedParticleString, EditorStyles.miniLabel);
        EditorGUILayout.Space();
        scaleMultiplier = EditorGUILayout.Slider("Scale Multiplier :", scaleMultiplier, 0.1f, 5.0f);

        if (GUILayout.Button("Scale", EditorStyles.miniButton)) {
            for (var gameObj : GameObject in Selection.gameObjects) {             
                var selectedParticles : ParticleSystem[];
                selectedParticles = gameObj.GetComponentsInChildren. < ParticleSystem > ();
                for (var thisParticle : ParticleSystem in selectedParticles) {
                    thisParticle.Stop();
                    scaleParticle(gameObj, thisParticle);
                    thisParticle.Play();
                }
            }
        }
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Play", EditorStyles.miniButtonLeft)) {
            playParticles();
        }
        if (GUILayout.Button("Pause", EditorStyles.miniButtonMid)) {
            pauseParticles();
        }
        if (GUILayout.Button("Stop", EditorStyles.miniButtonMid)) {
            stopParticles();
        }
        if(GUILayout.Button("Clear", EditorStyles.miniButtonRight)){
			clearParticles();
		}
        EditorGUILayout.EndHorizontal();
    }

	function playParticles() {
        for (var gameObj : GameObject in Selection.gameObjects) {
            var selectedParticles : ParticleSystem[];
            selectedParticles = gameObj.GetComponentsInChildren. < ParticleSystem > ();
            for (var thisParticle : ParticleSystem in selectedParticles) {
                thisParticle.Play();
            }
        }
    }
    
    function pauseParticles(){
    	for (var gameObj : GameObject in Selection.gameObjects) {
            var selectedParticles : ParticleSystem[];
            selectedParticles = gameObj.GetComponentsInChildren. < ParticleSystem > ();
            for (var thisParticle : ParticleSystem in selectedParticles) {
                thisParticle.Pause();
            }
        }
    }
    
    function stopParticles() {
        for (var gameObj : GameObject in Selection.gameObjects) {
            var selectedParticles : ParticleSystem[];
            selectedParticles = gameObj.GetComponentsInChildren. < ParticleSystem > ();
            for (var thisParticle : ParticleSystem in selectedParticles) {
            	thisParticle.Pause();
                thisParticle.Stop();
            }
        }
    }

    function clearParticles(){
    	for (var gameObj : GameObject in Selection.gameObjects) {
            var selectedParticles : ParticleSystem[];
            selectedParticles = gameObj.GetComponentsInChildren. < ParticleSystem > ();
            for (var thisParticle : ParticleSystem in selectedParticles) {
                thisParticle.Stop();
                thisParticle.Clear();
            }
        }
    }

    function scaleParticle(_gameObj : GameObject, _thisParticle : ParticleSystem) {
        if (_gameObj != _thisParticle.gameObject) {
            _thisParticle.transform.localPosition *= scaleMultiplier;
        }
        
        _thisParticle.startSize *= scaleMultiplier;
        _thisParticle.gravityModifier *= scaleMultiplier;
        _thisParticle.startSpeed *= scaleMultiplier;
        
        var serObj : SerializedObject = new SerializedObject(_thisParticle);
        serObj.FindProperty("ShapeModule.boxX").floatValue *= scaleMultiplier;
        serObj.FindProperty("ShapeModule.boxY").floatValue *= scaleMultiplier;
        serObj.FindProperty("ShapeModule.boxZ").floatValue *= scaleMultiplier;
        serObj.FindProperty("ShapeModule.radius").floatValue *= scaleMultiplier;
        
        scaleACV(serObj.FindProperty("VelocityModule.x.minCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("VelocityModule.x.maxCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("VelocityModule.y.minCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("VelocityModule.y.maxCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("VelocityModule.z.minCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("VelocityModule.z.maxCurve").animationCurveValue);
        
        serObj.FindProperty("ClampVelocityModule.x.scalar").floatValue *= scaleMultiplier;
        serObj.FindProperty("ClampVelocityModule.y.scalar").floatValue *= scaleMultiplier;
        serObj.FindProperty("ClampVelocityModule.z.scalar").floatValue *= scaleMultiplier;
        serObj.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= scaleMultiplier;
        
        scaleACV(serObj.FindProperty("ClampVelocityModule.x.minCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("ClampVelocityModule.x.maxCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("ClampVelocityModule.y.minCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("ClampVelocityModule.y.maxCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("ClampVelocityModule.z.minCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("ClampVelocityModule.z.maxCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("ClampVelocityModule.magnitude.minCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("ClampVelocityModule.magnitude.maxCurve").animationCurveValue);
        
        scaleACV(serObj.FindProperty("ForceModule.x.minCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("ForceModule.x.maxCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("ForceModule.y.minCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("ForceModule.y.maxCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("ForceModule.z.minCurve").animationCurveValue);
        scaleACV(serObj.FindProperty("ForceModule.z.maxCurve").animationCurveValue);
        
        serObj.FindProperty("VelocityModule.x.scalar").floatValue *= scaleMultiplier;
        serObj.FindProperty("VelocityModule.y.scalar").floatValue *= scaleMultiplier;
        serObj.FindProperty("VelocityModule.z.scalar").floatValue *= scaleMultiplier;
        
        serObj.FindProperty("ForceModule.x.scalar").floatValue *= scaleMultiplier;
        serObj.FindProperty("ForceModule.y.scalar").floatValue *= scaleMultiplier;
        serObj.FindProperty("ForceModule.z.scalar").floatValue *= scaleMultiplier;
        
        serObj.ApplyModifiedProperties();
    }

    function scaleACV(curve : AnimationCurve) {
        for (var i : int = 0; i < curve.keys.Length; i++) {
            curve.keys[i].value *= scaleMultiplier;
        }
    }
}