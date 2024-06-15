import SMPopUp from '@components/sm/SMPopUp';
import { getRandomColorHex } from '@lib/common/colors';
import Sketch from '@uiw/react-color-sketch';
import { OverlayPanel } from 'primereact/overlaypanel';
import { useEffect, useMemo, useRef, useState } from 'react';

interface ColorEditorProperties {
  readonly color: string;
  readonly editable?: boolean;
  readonly label?: string;
  onChange?(event: string): void;
}
const ColorEditor = ({ color: clientColor, editable = true, label, onChange }: ColorEditorProperties) => {
  const op = useRef<OverlayPanel>(null);
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
        className="color-editor-box flex justify-content-between align-items-center w-full px-1"
        style={{
          backgroundColor: color
        }}
      >
        <i className="flex justify-content-center align-items-center sm-button pi pi-chevron-circle-down" onClick={(e) => op.current?.toggle(e)} />
        <i
          className="flex justify-content-center align-items-center sm-button pi pi-refresh"
          onClick={(e) => {
            const c = getRandomColorHex();
            setColor(c);
            onChange && onChange(c);
          }}
        />
      </div>
    );
  }, [color, onChange]);

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

      <SMPopUp buttonTemplate={buttonTemplate} zIndex={10} showRemember={false}>
        <div className="w-full">
          <Sketch
            style={{ width: '100%' }}
            color={color}
            onChange={(color) => {
              setColor(color.hex);
              onChange && onChange(color.hex);
            }}
          />
        </div>
      </SMPopUp>
    </>
  );
};

export default ColorEditor;
