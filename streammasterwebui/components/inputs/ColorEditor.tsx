import SMButton from '@components/sm/SMButton';
import SMPopUp from '@components/sm/SMPopUp';
import { getRandomColorHex } from '@lib/common/colors';
import Sketch from '@uiw/react-color-sketch';
import { useEffect, useMemo, useState } from 'react';

interface ColorEditorProperties {
  readonly color: string;
  readonly editable?: boolean;
  readonly label?: string;
  onChange?(event: string): void;
}
const ColorEditor = ({ color: clientColor, editable = true, label, onChange }: ColorEditorProperties) => {
  const [color, setColor] = useState<string | undefined>(undefined);

  useEffect(() => {
    if (clientColor !== undefined) {
      if (color === undefined) {
        if (clientColor === '') {
          const c = getRandomColorHex();
          onChange && onChange(c);
        } else if (color !== clientColor) {
          setColor(clientColor);
        }
      }
    }
  }, [clientColor, color, onChange]);

  const buttonTemplate = useMemo(() => {
    return (
      <div
        className="color-editor-box flex justify-content-end align-items-center w-full px-1"
        style={{
          backgroundColor: color
        }}
      ></div>
    );
  }, [color]);

  if (editable !== undefined && !editable) {
    return (
      <div
        className="color-editor-box flex justify-content-between align-items-center"
        style={{
          backgroundColor: clientColor
        }}
      />
    );
  }

  return (
    <>
      {label && (
        <>
          <label className="pl-15">{label.toUpperCase()}</label>
        </>
      )}

      <SMPopUp buttonTemplate={buttonTemplate} showRemember={false} zIndex={12}>
        <div className="w-full flex justify-content-center flex-column align-content-center justify-items-center align-items-center gap-2">
          <Sketch
            style={{ width: '100%' }}
            color={color}
            onChange={(color) => {
              setColor(color.hex);
              onChange && onChange(color.hex);
            }}
          />
          <SMButton
            buttonClassName="sm-w-8rem icon-blue"
            icon="pi-refresh"
            iconFilled
            label="Random Color"
            onClick={() => {
              setColor(undefined);
              onChange && onChange('');
            }}
          />
        </div>
      </SMPopUp>
    </>
  );
};

export default ColorEditor;
