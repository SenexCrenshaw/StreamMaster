import { SMTextColor } from '@components/SMTextColor';

type GetLineProps = {
  label: string;
  value: React.ReactElement;
  help?: string | null;
  defaultSetting?: string | null;
};

export function getLine({ label, value, help, defaultSetting }: GetLineProps): React.ReactElement {
  return (
    <div className="flex col-12 align-content-center">
      <div className="flex col-2 col-offset-1">{label}</div>
      <div className="flex col-3 m-0 p-0 debug">{value}</div>
      {help !== null && help !== undefined && (
        <div className="flex flex-column col-3 text-sm align-content-center col-offset-1 debug">
          {help}
          {defaultSetting && <SMTextColor italicized text={defaultSetting} />}
        </div>
      )}
    </div>
  );
}
