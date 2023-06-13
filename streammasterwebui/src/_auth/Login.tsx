/* eslint-disable react/no-unused-prop-types */
/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/consistent-type-imports */

import React from "react";
import * as StreamMasterApi from '../store/iptvApi';
import * as Hub from '../store/signlar_functions';
import { Toast } from 'primereact/toast';
import { InputText } from "primereact/inputtext";
import { Checkbox } from "primereact/checkbox";
import { Button } from "primereact/button";

const Login = (props: LoginProps) => {
  const [checked, setChecked] = React.useState(true);

  return (

    <div className="flex align-items-center justify-content-center">
      <div className="surface-card p-4 shadow-2 border-round w-full lg:w-6">
        <div className="text-center mb-5">
          <img alt="hyper" className="mb-3" height={50} src="/demo/images/blocks/logos/hyper.svg" />
          <div className="text-900 text-3xl font-medium mb-3">Welcome Back</div>
          <span className="text-600 font-medium line-height-3">Don't have an account?</span>
          <a className="font-medium no-underline ml-2 text-blue-500 cursor-pointer">Create today!</a>
        </div>

        <div>
          <label className="block text-900 font-medium mb-2" htmlFor="email">Email</label>
          <InputText className="w-full mb-3" id="email" placeholder="Email address" type="text" />

          <label className="block text-900 font-medium mb-2" htmlFor="password">Password</label>
          <InputText className="w-full mb-3" id="password" placeholder="Password" type="password" />

          <div className="flex align-items-center justify-content-between mb-6">
            <div className="flex align-items-center">
              <Checkbox checked={checked} className="mr-2" id="rememberme" onChange={e => setChecked(e.checked as boolean)} />
              <label htmlFor="rememberme">Remember me</label>
            </div>
            <a className="font-medium no-underline ml-2 text-blue-500 text-right cursor-pointer">Forgot your password?</a>
          </div>

          <Button className="w-full" icon="pi pi-user" label="Sign In" />
        </div>
      </div>
    </div>

  );
}

Login.displayName = 'Login';
Login.defaultProps = {
  onChange: null,
  value: null,
};

type LoginProps = {
  data?: StreamMasterApi.ChannelGroupDto | undefined;
  onChange?: ((value: string) => void) | null;
  value?: string | null;
};

export default React.memo(Login);
