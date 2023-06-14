
import React from "react";

import { InputText } from "primereact/inputtext";
import { Checkbox } from "primereact/checkbox";
import { Button } from "primereact/button";
import * as Hub from '../store/signlar_functions';
import type * as StreamMasterApi from '../store/iptvApi';
import InfoMessageOverLayDialog from "../components/InfoMessageOverLayDialog";
import { useNavigate } from "react-router-dom";
import { type UserInformation } from "../common/common";
import { GetMessage } from "../common/common";
import { useLocalStorage, useSessionStorage } from "primereact/hooks";


const Login = (props: LoginProps) => {
  const [checked, setChecked] = React.useState(true);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [userName, setUserName] = React.useState('');
  const [password, setPassword] = React.useState('');
  const [block, setBlock] = React.useState<boolean>(false);
  const [isAuthenticated, setIsAuthenticated] = React.useState<boolean>(false);

  const [navigateTo,] = useSessionStorage<string>('/', 'navigateTo');
  const [, setUserInformation] = useLocalStorage<UserInformation>({} as UserInformation, 'userInformation');

  const navigate = useNavigate();


  const signInSuccessful = GetMessage('signInSuccessful');
  const signInUnSuccessful = GetMessage('signInUnSuccessful');

  const ReturnToParent = () => {
    setInfoMessage('');
    setBlock(false);
    props.onClose(isAuthenticated);

    navigate(navigateTo);

  };

  const onLogin = React.useCallback(async () => {

    setBlock(true);
    if (!userName || !password) {
      setBlock(false);
      return;
    }

    const data = {} as StreamMasterApi.LogInRequest;

    data.userName = userName;
    data.password = password;

    await Hub.LogIn(data)
      .then((result) => {
        console.log("onLogin: ", result);
        if (result) {

          setIsAuthenticated(true);
          setUserInformation({
            IsAuthenticated: true,
            TokenAge: new Date(),
          });
          setInfoMessage(signInSuccessful);
        } else {
          setInfoMessage(signInUnSuccessful);
        }
      }).catch(() => {
        setInfoMessage(signInUnSuccessful);
      });

    setBlock(false);
  }, [password, setUserInformation, signInSuccessful, signInUnSuccessful, userName]);

  return (
    <InfoMessageOverLayDialog
      blocked={block}
      closable={false}
      infoMessage={infoMessage}
      maximizable={false}
      onClose={() => { ReturnToParent(); }}
      overlayColSize={3}
      severity={isAuthenticated ? 'success' : 'error'}
      show
    >

      <div className="text-center mb-5">
        <img alt="hyper" className="mb-3" height={50} src="/images/StreamMasterx32.png" />
        <div className="text-900 text-3xl font-medium mb-3">Stream Master</div>
      </div>

      <div>
        <label className="block text-900 font-medium mb-2" htmlFor="user">User</label>
        <InputText className="w-full mb-3" id="user" onChange={(e) => setUserName(e.target.value)} placeholder={GetMessage("user")} type="text" value={userName} />

        <label className="block text-900 font-medium mb-2" htmlFor="password">Password</label>
        <InputText className="w-full mb-3" id="password" onChange={(e) => setPassword(e.target.value)} placeholder={GetMessage("password")} type="password" value={password} />

        <div className="flex align-items-center justify-content-between mb-6">
          <div className="flex align-items-center">
            <Checkbox checked={checked} className="mr-2" id="rememberme" onChange={e => setChecked(e.checked as boolean)} />
            <label htmlFor="rememberme">{GetMessage("rememberme")}</label>
          </div>
        </div>

        <Button className="w-full" disabled={(!userName || !password)} icon="pi pi-user" label={GetMessage("signin")} onClick={async () => await onLogin()} />

      </div>
    </InfoMessageOverLayDialog>

  );
}

Login.displayName = 'Login';
Login.defaultProps = {

};

type LoginProps = {
  onClose: (isAuthenticated: boolean) => void;
};


export default React.memo(Login);
