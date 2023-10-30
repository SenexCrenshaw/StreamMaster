import React from 'react';

interface InputWrapperProperties {
  readonly columnSize: number
  readonly label: string
  readonly renderInput: () => React.ReactNode
}

const InputWrapper: React.FC<InputWrapperProperties> = ({
  columnSize,
  label,
  renderInput
}) => (
  <div
    className={`flex flex-wrap col-${columnSize} justify-content-start align-items-center p-0 m-0`}
  >
    <div className="flex col-12 p-0 m-0 pl-1 text-xs">{label}</div>
    <div className="flex col-12 justify-content-start align-items-center p-0 m-0">
      {renderInput()}
    </div>
  </div>
);

export default InputWrapper;
