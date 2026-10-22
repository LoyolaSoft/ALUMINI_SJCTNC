let tensorVideo = document.getElementById('webcam');
let objectDetection = document.getElementById('objectDetection');
let cellPhoneTimer = 0;
let laptopTimer = 0;
let multiplePersonTimer = 0;
var children = [];
var scale = 0.25;
var camera=$('#camera').val();
var log=$('#log').val();

// Store the resulting coco-ssd model in the global scope of our app.
if(camera=="1")
{
    var model = undefined;
    if (!model) {
        $('#setting-modal').modal('toggle');
    }
    // Before we can use COCO-SSD class we must wait for it to finish
    // loading. Machine Learning models can be large and take a moment 
    // to get everything needed to run.
    // Note: cocoSsd is an external object loaded from our index.html
    // script tag import so ignore any warning in Glitch.
    cocoSsd.load().then(function (loadedModel) {
        model = loadedModel;
        $('#setting-modal').modal('toggle');
        $('#permission-modal').modal('toggle');
        // Show demo section now model is ready to use.
        //  demosSection.classList.remove('invisible');
    });
}

    //Getting permission
    // Check if webcam access is supported.
    function getUserMediaSupported() {
        //  return !!(navigator.mediaDevices &&
        //    navigator.mediaDevices.getUserMedia);

        navigator.getUserMedia = (navigator.getUserMedia ||
            navigator.webkitGetUserMedia ||
            navigator.mozGetUserMedia ||
            navigator.msGetUserMedia);
        return !!(navigator.getUserMedia);
    }

function requestGeolocation() {
    navigator.geolocation.getCurrentPosition(locationStatus);
}
function requestCamera() {
    if (typeof navigator.mediaDevices.getUserMedia === 'undefined') {
        navigator.getUserMedia({ video: { facingMode: 'user' } }, () => {
            checkPermission('camera');
        }, noop);
    } else {
        navigator.mediaDevices.getUserMedia({ video: { facingMode: 'user' } }).then(() => {
            //checkPermission('camera');
            document.getElementById('camera-status').innerHTML = 'granted';
            enableModalButton();
        }).catch(() => { });
    }

}

var noop = function () {

};
var locationStatus = function (pos) {
    checkPermission('geolocation');
    var crd = pos.coords;
    $('#hdlatitude').val(crd.latitude);
    $('#hdLongitude').val(crd.longitude);
};
var enableModalButton = () => {
    if ($('#camera-status').text() == 'granted') {
        $('#btnModalOK').removeClass('hidden');
    }
};
// If webcam supported, add event listener to button for when user
// wants to activate it to call enableCam function which we will 
// define in the next step.
function callDetection(videoTag, objectTag) {

    if (getUserMediaSupported()) {
        enableCam(videoTag, objectTag)
    } else {
        console.warn('getUserMedia() is not supported by your browser');
    }
}


// Placeholder function for next step. Paste over this in the next step.
// Enable the live webcam view and start classification.
function enableCam(videoTag, objectTag) {
    // Only continue if the COCO-SSD has finished loading.
    if (!model) {
        return;
    }
    tensorVideo = videoTag;
    objectDetection = objectTag;
    // Hide the button once clicked.
    //event.target.classList.add('removed');

    // getUsermedia parameters to force video but not audio.
    const constraints = {
        video: true
    };

    // Activate the webcam stream.
    // navigator.mediaDevices.getUserMedia({ video: { facingMode: 'user' } }).then( function (stream) {
    //    window.stream = stream;
    // tensorVideo.srcObject = stream;	
    //    tensorVideo.addEventListener('loadeddata', predictWebcam);	 
    // });
    predictWebcam();
}
function predictWebcam() {
    // Now let's start classifying a frame in the stream.
    model.detect(tensorVideo).then(function (predictions) {
        // Remove any highlighting we did previous frame.
        objectDetection.innerText = '';
        children.splice(0);
        // Now lets loop through predictions and draw them to the live view if
        // they have a high confidence score.      
        for (let n = 0; n < predictions.length; n++) {
            // If we are over 66% sure we are sure we classified it right, draw it!
            if (predictions[n].score > 0.66) {
                const p = document.createElement('p');
                children.push(predictions[n].class);
            }
        }
        capture(children);
        objectDetection.innerText = children.toString();
        // Call this function again to keep predicting when the browser is ready.
        window.requestAnimationFrame(predictWebcam);
    });
}
const countOccurrences = (arr, val) => arr.reduce((a, v) => (v === val ? a + 1 : a), 0);

async function capture(input) {
    if (input.length > 0) {
        let cellPhoneCount = countOccurrences(input, 'cell phone');
        let personCount = countOccurrences(input, 'person');
        let laptopCount = countOccurrences(input, 'laptop');
        if (cellPhoneCount > 0) {
            if (cellPhoneTimer == 0) {
                await captureImage('Detected Cell Phone', '1');
                cellPhoneTimer = 60;
            }
        }
        if (personCount > 1) {
            if (multiplePersonTimer == 0) {
                await captureImage('Detected Multiple Person', '1');
                multiplePersonTimer = 60;
            }
        } if (laptopCount > 0) {
            if (laptopTimer == 0) {
              //  await captureImage('Detected Laptop', '1');
                laptopTimer = 60;
            }
        }
    }
}

var captureImage = async function (Description, Violation) 
{
   
    if (tensorVideo) 
    {
        var canvas = document.createElement("canvas");
        canvas.width = tensorVideo.videoWidth * scale;
        canvas.height = tensorVideo.videoHeight * scale;
        canvas.getContext('2d').drawImage(tensorVideo, 0, 0, canvas.width, canvas.height);
        var img = document.createElement("img");
        await examStudentLog(Description, Violation, canvas.toDataURL());
    }
    if(log=="1" && camera=="0")
    {
        await examStudentLog(Description, Violation, "");
    }
    
   
};
function timer() {
    setInterval(function () {
        if (cellPhoneTimer > 0) {
            cellPhoneTimer = cellPhoneTimer - 1;
        }
        if (laptopTimer > 0) {
            laptopTimer = laptopTimer - 1;
        } if (multiplePersonTimer > 0) {
            multiplePersonTimer = multiplePersonTimer - 1;
        }
    }, 1000);
}
timer();
//to check permission
function checkPermission(permissionName, descriptor) {
    try {
        navigator.permissions.query(Object.assign({ name: permissionName }, descriptor))
            .then(function (permission) {
                document.getElementById(permissionName + '-status').innerHTML = permission.state;
                permission.addEventListener('change', function (e) {
                    document.getElementById(permissionName + '-status').innerHTML = permission.state;
                    // handleChange(permissionName, permission.state);
                });
                enableModalButton();
            });

    } catch (e) {
    }
}
function examStudentLog(DESCRIPTION, IS_VIOLATION, IMAGE_PATH) 
{
  
    var postData = {};
    postData.STUDENT_ID = $('#userid').val();
    postData.INVIGILATION_ID = $('#hdInvigilationId').val();
    postData.EXAM_ID = $('#hdExamSettingId').val();
    postData.COURSE_ID = $('#hdExamCourseId').val();
    postData.PUBLIC_IP = $('#hdClientIp').val();
    postData.LATITUDE = $('#hdlatitude').val();
    postData.LONGITUDE = $('#hdLongitude').val();
    postData.IS_VIOLATION = IS_VIOLATION;
    postData.DESCRIPTION = DESCRIPTION;
    postData.IMAGE_PATH = IMAGE_PATH;
    postData.ACADEMIC_YEAR = $('#acyear').val();
    postData.DEVICE = navigator.userAgent;
    $.ajax({
        url: "/Examination/ExamStudentLog",
        method: 'post',
        data: { sLogInfo: JSON.stringify(postData) }
        , success: function (result) {
            console.log(result);
        }, error: (e) => {
            console.log(e);
        }

    });

}


function handleVisibilityChange() {
    if (document.hidden) {
        swal({ title: 'It is a violation , you are not allowed leave this page during the examination', icon: "error" });
        captureImage('Student left the examination page.', '1');
    } else {

    }
}
//visibility change
document.addEventListener("visibilitychange", handleVisibilityChange, false);
//mouse event
$(document).bind("mouseleave", function (e) {
    if (e.pageY - $(window).scrollTop() <= 1) {
        swal({ title: 'You are not allowed to leave this page', icon: 'error' });
    }
});
//handling copy and cut event
window.onload = function () {
    document.addEventListener('copy', function (e) {
        selectedText = "You are not allowed to copy or cut !!";
        if (window.clipboardData) {
            window.clipboardData.setData("Text", selectedText);
        } else {
            e.clipboardData.setData('text/plain', selectedText);
        }
        swal({ title: 'You are not allowed to copy or cut !!', icon: 'error' });
        captureImage('Student is trying to copy or cut from examination page.', '1');
        e.preventDefault();
    });
}
$('#hdUserAgent').val(navigator.userAgent);
function isMobile() {
    return /iPhone|iPad|iPod|Android/i.test(navigator.userAgent);
}



