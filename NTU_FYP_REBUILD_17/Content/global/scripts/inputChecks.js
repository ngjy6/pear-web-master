function languageCheck() {
    //Check if language is Others
    $("#langCheck").change(function () {
        var val = document.getElementById('langCheck').value;
        var element = document.getElementById('preferredLanguageOther');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';

    });
}

function relationshipCheck() {
//Check if Relationship is Others
$("#rsCheck").change(function () {
    var val = document.getElementById('rsCheck').value;
    var element = document.getElementById('otherRelation');
    if (val == -1)
        element.style.display = 'block';
    else
        element.style.display = 'none';

});

$("#rs2Check").change(function () {
    var val = document.getElementById('rs2Check').value;
    var element = document.getElementById('other2Relation');
    if (val == -1)
        element.style.display = 'block';
    else
        element.style.display = 'none';

    });
}

//function dementiaCheck() {
//    //Check if Dementia Type is Others
//    $("#dementiaCheck").change(function () {
//        var val = document.getElementById('dementiaCheck').value;
//        var element = document.getElementById('otherDementia');
//        if (val == -1)
//            element.style.display = 'block';
//        else
//            element.style.display = 'none';

//    });
//}

//Check if SocialHistory is Others
//Mobility
function socialHistoryCheck() {
$("#mobilityCheck").change(function () {
    var val = document.getElementById('mobilityCheck').value;
    var element = document.getElementById('otherMobility');
    if (val == -1)
        element.style.display = 'block';
    else
        element.style.display = 'none';

});

$("#liveWithCheck").change(function () {
    var val = document.getElementById('liveWithCheck').value;
    var element = document.getElementById('otherLiveWith');
    if (val == -1)
        element.style.display = 'block';
    else
        element.style.display = 'none';

});

$("#religionCheck").change(function () {
    var val = document.getElementById('religionCheck').value;
    var element = document.getElementById('otherReligion');
    if (val == -1)
        element.style.display = 'block';
    else
        element.style.display = 'none';

});

$("#educationCheck").change(function () {
    var val = document.getElementById('educationCheck').value;
    var element = document.getElementById('otherEducation');
    if (val == -1)
        element.style.display = 'block';
    else
        element.style.display = 'none';

    });

    $("#occupationCheck").change(function () {
        var val = document.getElementById('occupationCheck').value;
        var element = document.getElementById('otherOccupation');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';

    });

    $("#petCheck").change(function () {
        var val = document.getElementById('petCheck').value;
        var element = document.getElementById('otherPet');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';

    });

    $("#dietCheck").change(function () {
        var val = document.getElementById('dietCheck').value;
        var element = document.getElementById('otherDiet');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';

    });
}

function preferencesCheck() {
    $("#likeVal1").change(function () {
        var val = document.getElementById('likeVal1').value;
        var element = document.getElementById('preferenceOther');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';
    });

    $("#dislikeVal1").change(function () {
        var val = document.getElementById('dislikeVal1').value;
        var element = document.getElementById('preferenceOther');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';
    });

    $("#dislikeVal").change(function () {
        var val = document.getElementById('dislikeVal').value;
        var element = document.getElementById('preferenceOther1');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';
    });

    $("#likeVal").change(function () {
        var val = document.getElementById('likeVal').value;
        console.log(val);
        var element = document.getElementById('preferenceOther1');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';
    });

}

//nric
function nricValidation(nric, type) {
    var regex1 = RegExp('^[a-zA-Z]{1}[0-9]{7}[a-zA-Z]$');
       
    var str = nric;
    var res = str.substring(1, 8);

    res7 = res % 10; res = parseInt(res / 10);
    res6 = res % 10; res = parseInt(res / 10);
    res5 = res % 10; res = parseInt(res / 10);
    res4 = res % 10; res = parseInt(res / 10);
    res3 = res % 10; res = parseInt(res / 10);
    res2 = res % 10; res = parseInt(res / 10);
    res1 = res % 10;

    sum = (res7 * 2) + (res6 * 3) + (res5 * 4) + (res4 * 5) + (res3 * 6) + (res2 * 7) + (res1 * 2);

    remainder = sum % 11;
    final = 11 - remainder;

    var first = nric.charAt(0);

    var listOfChar;
    if (first == 'S' || first == 's') {
        listOfChar = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'Z', 'J'];

    } else if (first == 'T' || first == 't') {
        listOfChar = ['H', 'I', 'Z', 'J', 'A', 'B', 'C', 'D', 'E', 'F', 'G'];
    } else {

        if (type == "NRIC") {
            document.getElementById("nricError").innerHTML = "Please enter a valid NRIC !";
            document.getElementById("nricError").style.display = "block";

            return false;

        } else if (type == "NRICg") {
            document.getElementById("nricErrorG").innerHTML = "Please enter a valid NRIC !";
            document.getElementById("nricErrorG").style.display = "block";
            return false;

        } else if (type == "NRICg2") {
            document.getElementById("nricErrorG").innerHTML = "Please enter a valid NRIC !";
            document.getElementById("nricErrorG").style.display = "block";
            return false;
        }

    } 


    var code = ' ';
    code = listOfChar[final - 1];
    var errorFlag = 0;

    if (type == "NRIC") {
        if (regex1.test(nric) == false) {
            document.getElementById("nricError").innerHTML = "Please enter a valid NRIC !";
            document.getElementById("nricError").style.display = "block";
            errorFlag = 1;
        }

        if (str.substr(8, 1).valueOf().toUpperCase() != code.valueOf().toUpperCase()) {
            document.getElementById("nricError").innerHTML = "Please enter a valid NRIC !";
            document.getElementById("nricError").style.display = "block";
            errorFlag = 1;
        } 

        if (errorFlag == 1) {
            return false;
        }

    }

    else if (type == "NRICg") {
        if (regex1.test(nric) == false) {
            document.getElementById("nricErrorG").innerHTML = "Please enter a valid NRIC !";
            document.getElementById("nricErrorG").style.display = "block";
            errorFlag = 1;
        }

        if (str.substr(8, 1).valueOf().toUpperCase() != code.valueOf().toUpperCase()) {
            document.getElementById("nricErrorG").innerHTML = "Please enter a valid NRIC !";
            document.getElementById("nricErrorG").style.display = "block";

            errorFlag = 1;
        } 


        if (errorFlag == 1) {
            return false;
        }
    }

    else if (type == "NRICg2") {
        if (regex1.test(nric) == false) {
            document.getElementById("nricErrorG2").innerHTML = "Please enter a valid NRIC !";
            document.getElementById("nricErrorG2").style.display = "block";
            errorFlag = 1;
        }

        if (str.substr(8, 1).valueOf().toUpperCase() != code.valueOf().toUpperCase()) {
            document.getElementById("nricErrorG2").innerHTML = "Please enter a valid NRIC !";
            document.getElementById("nricErrorG2").style.display = "block";
            errorFlag = 1;
        }

        if (errorFlag == 1) {
            return false;
        }
    }


    document.getElementById("nricError").innerHTML = "";
    document.getElementById("nricError").style.display = "none";

    document.getElementById("nricErrorG").innerHTML = "";
    document.getElementById("nricErrorG").style.display = "none";

    document.getElementById("nricErrorG2").innerHTML = "";
    document.getElementById("nricErrorG2").style.display = "none";

        return true;
    
}

//prescription Check
function prescriptionCheck() {
    $("#drugNameCheck").change(function () {
        var val = document.getElementById('drugNameCheck').value;
        var element = document.getElementById('drugNameOther');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';

    });

    $("#drugName").change(function () {
        var val = document.getElementById('drugName').value;
        var element = document.getElementById('drugNameOther1');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';

    });

};

//allergyCheck
function allergyCheck() {
    $("#allergyNameCheck").change(function () {
        var val = document.getElementById('allergyNameCheck').value;
        var element = document.getElementById('allergyNameOther');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';

    });

    $("#allergyNameCheck1").change(function () {
        var val = document.getElementById('allergyNameCheck1').value;
        var element = document.getElementById('allergyNameOther1');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';

    });

};


//routineCheck
function routineCheck() {
    $("#activityCheck").change(function () {
        var val = document.getElementById('activityCheck').value;
        var element = document.getElementById('activityOther');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';

    });


    $("#activityCheck1").change(function () {
        var val = document.getElementById('activityCheck1').value;
        var element = document.getElementById('activityOther1');
        if (val == -1)
            element.style.display = 'block';
        else
            element.style.display = 'none';

    });

};

function dateCheckaddPatient() {

    var datetimepicker1Val = $("#datetimepicker1").val();
    var datetimepicker2Val = $("#datetimepicker2").val();

    var datetime = new Date();
    var dd = datetime.getDate();
    var mm = datetime.getMonth() + 1; //January is 0!

    var yyyy = datetime.getFullYear();
    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }
    var d = dd + '/' + mm + '/' + yyyy;


    var arrStartDate = datetimepicker1Val.split("/");
    var stdate = new Date(arrStartDate[2], arrStartDate[1], arrStartDate[0]);

    var arrEndDate = datetimepicker2Val.split("/");
    var enddate = new Date(arrEndDate[2], arrEndDate[1], arrEndDate[0]);


    var arrCrDate = d.split("/");
    var tdate = new Date(arrCrDate[2], arrCrDate[1], arrCrDate[0]);

    var errorFlag = 0;
    if (stdate < tdate) {
        document.getElementById("startDateError").innerHTML = "Start date cannot be before today.";
        document.getElementById("startDateError").style.display = "block";
        errorFlag = 1;

    } else {
        document.getElementById("startDateError").innerHTML = "";
        document.getElementById("startDateError").style.display = "none";
    }

    if (stdate.toDateString() == enddate.toDateString() && enddate != "") {
        document.getElementById("startDateError2").innerHTML = "Start date and end date cannot be on the same day.";
        document.getElementById("startDateError2").style.display = "block";
        errorFlag = 1;
    } else {
        document.getElementById("startDateError2").innerHTML = "";
        document.getElementById("startDateError2").style.display = "none";
    }

    if (stdate > enddate && enddate != "") {
        document.getElementById("startDateError1").innerHTML = "Start date cannot be after end date.";
        document.getElementById("startDateError1").style.display = "block";
        errorFlag = 1;

    } else {
        document.getElementById("startDateError1").innerHTML = "";
        document.getElementById("startDateError1").style.display = "none";
    }


    if (enddate < tdate && stdate != "") {
        document.getElementById("endDateError").innerHTML = "End date cannot be before today.";
        document.getElementById("endDateError").style.display = "block";
        errorFlag = 1;

    } else {
        document.getElementById("endDateError").innerHTML = "";
        document.getElementById("endDateError").style.display = "none";
    }

    if (errorFlag == 1) {
        return false;
    } else {
        return true;
    }

}

function fieldValidityCheck() {
    var curStep = $(this).closest(".tab-pane");

    var curInputs = curStep.find("input[type='text'],input[type='url']");

    for (var i = 0; i < curInputs.length; i++) {
   
    if (!curInputs[i].validity.valid) {
        isValid = false;
        $(curInputs[i]).closest(".form-group").addClass("has-error");
    }

    }
}
