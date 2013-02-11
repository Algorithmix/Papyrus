var labelType, useGradients, nativeTextSupport, animate, st;

(function() {
  var ua = navigator.userAgent,
      iStuff = ua.match(/iPhone/i) || ua.match(/iPad/i),
      typeOfCanvas = typeof HTMLCanvasElement,
      nativeCanvasSupport = (typeOfCanvas == 'object' || typeOfCanvas == 'function'),
      textSupport = nativeCanvasSupport 
        && (typeof document.createElement('canvas').getContext('2d').fillText == 'function');
  //I'm setting this based on the fact that ExCanvas provides text support for IE
  //and that as of today iPhone/iPad current text support is lame
  labelType = (!nativeCanvasSupport || (textSupport && !iStuff))? 'Native' : 'HTML';
  nativeTextSupport = labelType == 'Native';
  useGradients = nativeCanvasSupport;
  animate = !(iStuff || !nativeCanvasSupport);
})();

var Log = {
  elem: false,
  write: function(text){
    if (!this.elem) 
      this.elem = document.getElementById('log');
    this.elem.innerHTML = text;
    this.elem.style.left = (100) + 'px';
  }
};


function init(){
    //init data
    
    //end
    //init Spacetree
    //Create a new ST instance
    st = new $jit.ST({
		levelsToShow: 100,
        //id of viz container element
        injectInto: 'infovis',
        //set duration for the animation
        duration: 400,
        //set animation transition type
        transition: $jit.Trans.Quart.easeInOut,
        //set distance between node and its children
        levelDistance: 60,
        //enable panning
        Navigation: {
          enable:true,
          panning:true
        },
        //set node and edge styles
        //set overridable=true for styling individual
        //nodes or edges
        Node: {
            height: 30,
            width: 60,
            type: 'rectangle',
            color: '#666',
            overridable: true
        },
        
        Edge: {
            type: 'bezier',
            overridable: true,
            lineWidth: 2,  
            color:'#23A4FF'
        },
        Tips: {  
          enable: true,  
          type: 'auto',  
          offsetX: 10,  
          offsetY: 10,  
          onShow: function(tip, node) {  
            tip.innerHTML = node.data.tip;  
          } 
        },
        
        onBeforeCompute: function(node){
            Log.write("loading " + node.name);
        },
        
        onAfterCompute: function(){
            Log.write("done");
        },
        
        //This method is called on DOM label creation.
        //Use this method to add event handlers and styles to
        //your node.
        onCreateLabel: function(label, node){
            label.id = node.id;            
            label.innerHTML = node.name;
            label.onclick = function(){st.onClick(node.id)};
            //set label styles
            var style = label.style;
            style.width = 60 + 'px';
            style.height = 17 + 'px';            
            style.cursor = 'pointer';
            style.color = '#fff';
            style.fontWeight = "bold";
            style.fontSize = '0.8em';
            style.textAlign= 'center';
            style.paddingTop = '3px';
        },
        
        //This method is called right before plotting
        //a node. It's useful for changing an individual node
        //style properties before plotting it.
        //The data properties prefixed with a dollar
        //sign will override the global node style properties.
        onBeforePlotNode: function(node){
            //add some color to the nodes in the path between the
            //root node and the selected node.
            if (node.selected) {
                node.data.$color = "#00CD00";
            }
            else {
                delete node.data.$color;
                //if the node belongs to the last plotted level
                if(!node.anySubnode("exist")) {
                    //count children number
                    var count = 0;
                    node.eachSubnode(function(n) { count++; });
                    //assign a node color based on
                    //how many children it has
                    node.data.$color = ['#566D7E', '#243B4C', '#566D7E', '#243B4C', '#60557D','#566D7E'][count];                    
                }
            }
        },
        
        //This method is called right before plotting
        //an edge. It's useful for changing an individual edge
        //style properties before plotting it.
        //Edge data proprties prefixed with a dollar sign will
        //override the Edge global style properties.
        onBeforePlotLine: function(adj){
            if (adj.nodeFrom.selected && adj.nodeTo.selected) {
                adj.data.$color = "#eed";
                adj.data.$lineWidth = 3;
            }
            else {
                delete adj.data.$color;
                delete adj.data.$lineWidth;
            }
        }
    });
    //load json data
    st.loadJSON(json);
    //compute node positions and layout
    st.compute();
    //optional: make a translation of the tree
    st.geom.translate(new $jit.Complex(-200, 0), "current");
    //emulate a click on the root node.
    st.onClick(st.root);;  
    
    //end
    //Add event handlers to switch spacetree orientation.
    // var top = $jit.id('r-top'), 
    //     left = $jit.id('r-left'), 
    //     bottom = $jit.id('r-bottom'), 
    //     right = $jit.id('r-right'),
    //     normal = $jit.id('s-normal');
        
    
    // function changeHandler() {
    //     if(this.checked) {
    //         top.disabled = bottom.disabled = right.disabled = left.disabled = true;
    //         st.switchPosition(this.value, "animate", {
    //             onComplete: function(){
    //                 top.disabled = bottom.disabled = right.disabled = left.disabled = false;
    //             }
    //         });
    //     }
    // };
    
    // top.onchange = left.onchange = bottom.onchange = right.onchange = changeHandler;
    //end

}
