import { SMTextColor } from '@components/sm/SMTextColor';

type GetLineProps = {
  value: React.ReactElement;
  help?: string | null;
  defaultSetting?: string | null;
};

export function getLine({ value, help, defaultSetting }: GetLineProps): React.ReactElement {
  return (
    <div className="flex w-full justify-items-center align-items-center align-content-center settings-line">
      <div className="w-6 m-0 p-0 ">{value}</div>
      {help !== null && help !== undefined && (
        <>
          <div className="flex pl-2 w-full text-sm align-content-center">
            <div className="w-6 m-0 p-0">{help}</div>
            {defaultSetting && (
              <div className="w-6 m-0 p-0">
                <span>: </span>
                <SMTextColor italicized text={defaultSetting} />
              </div>
            )}
          </div>
        </>
      )}
    </div>
  );
}
