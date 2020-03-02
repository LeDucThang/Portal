sessionStorage.clear(); 
function submitLogin() {
    debugger;
    var isValid = true;
    if ($('#frmLogin input[name="Username"]').val() == '') {
        debugger;
        $('#UsernameErr').html('Username không được để trống');
        isValid = false;
    } 
    if ($('#frmLogin input[name="Password"]').val() == '') {
        $('#passwordErr').html('Mật khẩu không được để trống');
        isValid = false;
    } else {
        $('#passwordErr').html('');
    }

    var data = getFormData($('#frmLogin'));

    if (isValid) {
        $.ajax({
            method: "POST",
            url: "/Account/Login",
            data: { 
                data: { Username: data.Username, Password: data.Password} 
            }
        }).done(function (res) {
            debugger;
            if (res) {
                if (res.error) {
                    CreateMessage("error", "Thông báo", res.message);
                    $('#messageError').html(res.message);
                }
                else {
                    CreateMessage("success", "Thông báo", res.message);
                    sessionStorage.setItem("IsLogin", "loggedIn"); 
                    if (res.IsResetPass && res.IsResetPass.toString().toLowerCase() == "true") {
                        location.href = "/doi-mat-khau";
                    }
                    else {
                        location.href = res.returnUrl;
                    }

                }
            }
        });
    }
}


$('#Email', '#Password').keypress(function (event) {
    var keycode = (event.keyCode ? event.keyCode : event.which);
    if (keycode == '13') {
        submitLogin();
    }
});

$('#submitLogin').click(function (e) {
    e.preventDefault();
    submitLogin();
});
$('#frmLogin').on('keydown', function (e) {
    if (e.which == 13) {
        submitLogin();
        e.preventDefault();
    }
});

$('.btn-login-facebook').click(function (e) {
    e.preventDefault();

    FB.login(function (response) {
        if (response.status === 'connected') {
            document.location = '/Account/FacebookCallback?code=' + response.authResponse.accessToken;
        } else {
            alert('ERROR');
        }
    }, { scope: 'public_profile,email,user_photos,user_birthday,user_location' });

});

$('.btn-login-google').click(function (e) {
    //e.preventDefault(); 
    window.location = "https://accounts.google.com/o/oauth2/auth?client_id=998310595831-dceeoaikv8ce1qls0v35h1fbd3uskiel.apps.googleusercontent.com&state=" + $('#stateNew').val() + "&redirect_uri=" + "https://localhost:5001/account/GoogleCallback" + "&response_type=code&scope=https://www.googleapis.com/auth/userinfo.email";

});

// This is called with the results from from FB.getLoginStatus().
function statusChangeCallback(response) {
    //console.log('statusChangeCallback');
    //console.log(response);
    // The response object is returned with a status field that lets the
    // app know the current login status of the person.
    // Full docs on the response object can be found in the documentation
    // for FB.getLoginStatus().
    if (response.status === 'connected') {
        // Logged into your app and Facebook.
        testAPI();
    } else {
        // The person is not logged into your app or we are unable to tell.
        //document.getElementById('status').innerHTML = 'Please log ' +
        //    'into this app.';
    }
}

// This function is called when someone finishes with the Login
// Button.  See the onlogin handler attached to it in the sample
// code below.
function checkLoginState() {
    FB.getLoginStatus(function (response) {
        statusChangeCallback(response);
    });
}

window.fbAsyncInit = function () {
    FB.init({
        appId: '675579672952525',
        //appId: '2175852355985253',
        cookie: true,  // enable cookies to allow the server to access
        // the session
        xfbml: true,  // parse social plugins on this page
        version: 'v4.0' // use graph api version 2.8
    });

    // Now that we've initialized the JavaScript SDK, we call
    // FB.getLoginStatus().  This function gets the state of the
    // person visiting this page and can return one of three states to
    // the callback you provide.  They can be:
    //
    // 1. Logged into your app ('connected')
    // 2. Logged into Facebook, but not your app ('not_authorized')
    // 3. Not logged into Facebook and can't tell if they are logged into
    //    your app or not.
    //
    // These three cases are handled in the callback function.

    FB.getLoginStatus(function (response) {
        statusChangeCallback(response);
    });

};

// Load the SDK asynchronously
(function (d, s, id) {
    var js, fjs = d.getElementsByTagName(s)[0];
    if (d.getElementById(id)) return;
    js = d.createElement(s); js.id = id;
    js.src = "https://connect.facebook.net/en_US/sdk.js";
    fjs.parentNode.insertBefore(js, fjs);
}(document, 'script', 'facebook-jssdk'));

// Here we run a very simple test of the Graph API after login is
// successful.  See statusChangeCallback() for when this call is made.
function testAPI() {
    console.log('Welcome!  Fetching your information.... ');
    FB.api('/me', function (response) {
        console.log('Successful login for: ' + response.name);
        //document.getElementById('status').innerHTML =
        //    'Thanks for logging in, ' + response.name + '!';
    });
}


//Register

$(document).ready(function () {
    $('#submitRegister').click(function (e) {
        debugger;
        e.preventDefault();

        var data = getFormData($('#frmRegister'));
        data.IsReceiveEmail = data.IsReceiveEmail != undefined && data.IsReceiveEmail != null;

        var isValid = true, strFocus = '';
        if ($('#frmRegister input[name="Email"]').val() == '') {
            $('#emailErr').html('Email không được để trống');
            strFocus = strFocus == '' ? 'Email' : strFocus;
            isValid = false;
        } else if (!validateEmail($('#frmRegister input[name="Email"]').val())) {
            $('#emailErr').html('Địa chỉ email không hợp lệ');
            strFocus = strFocus == '' ? 'Email' : strFocus;
            isValid = false;
        } else {
            $('#emailErr').html('');
        }

        if ($('#frmRegister input[name="FullName"]').val() == '') {
            $('#fullNameErr').html('Vui lòng nhập họ tên đầy đủ');
            strFocus = strFocus == '' ? 'FullName' : strFocus;
            isValid = false;
        } else {
            $('#fullNameErr').html('');
        }

        if ($('#frmRegister input[name="Password"]').val() == '') {
            $('#passwordErr').html('Vui lòng nhập mật khẩu');
            strFocus = strFocus == '' ? 'Password' : strFocus;
            isValid = false;
        } else if (!validatePassword($('#frmRegister input[name="Password"]').val())) {
            $('#passwordErr').html('Mật khẩu phải có ít nhất 6 ký tự bao gồm số, chữ hoa, chữ thường và ký tự đặc biệt');
            strFocus = strFocus == '' ? 'Password' : strFocus;
            isValid = false;
        } else {
            $('#passwordErr').html('');
        }

        if ($('#frmRegister input[name="ConfirmPassword"]').val() == '') {
            $('#confirmPasswordErr').html('Mật khẩu xác nhận không được để trống');
            strFocus = strFocus == '' ? 'ConfirmPassword' : strFocus;
            isValid = false;
        } else if ($('#frmRegister input[name="ConfirmPassword"]').val() != $('#frmRegister input[name="Password"]').val()) {
            $('#confirmPasswordErr').html('Mật khẩu xác nhận không hợp lệ');
            strFocus = strFocus == '' ? 'ConfirmPassword' : strFocus;
            isValid = false;
        } else {
            $('#confirmPasswordErr').html('');
        }

        if ($('#frmRegister input[name="Phone"]').val() == '') {
            $('#phoneErr').html('Vui lòng nhập số điện thoại');
            strFocus = strFocus == '' ? 'Phone' : strFocus;
            isValid = false;
        } else if (!validatePhone($('#frmRegister input[name="Phone"]').val())) {
            $('#phoneErr').html('Số điện thoại không hợp lệ');
            strFocus = strFocus == '' ? 'Phone' : strFocus;
            isValid = false;
        } else {
            $('#phoneErr').html('');
        }

        if (!isValid) {
            $('#frmRegister input[name="' + strFocus + '"]').focus();
            return false;
        }

        $.ajax({
            method: "POST",
            url: "/Account/Register",
            data: {
                __RequestVerificationToken: data.__RequestVerificationToken,
                data: data
            }
        }).done(function (res) {
            debugger;
            if (res) {
                if (res.error) {
                    res.emailExisted ? ($('#emailErr').html('Địa chỉ email đã được sử dụng'), $('#Email').val('')) : null;
                    res.phoneExisted ? ($('#phoneErr').html('Số điện thoại đã được sử dụng'), $('#Phone').val('')) : null;
                    res.userNameExisted ? ($('#userNameErr').html('Tên đăng nhập đã được sử dụng'), $('#UserName').val('')) : null;
                    res.emailExisted ? $('#frmRegister input[name="Email"]').focus() : res.userNameExisted ? $('#frmRegister input[name="UserName"]').focus() : res.phoneExisted ? $('#frmRegister input[name="Phone"]').focus() : null;
                } else {
                    CreateMessage("success", "Thông báo", res.message);
                    setTimeout(function () {
                        location.href = '/';
                    }, 1000);

                }
            }
        });

    });
});