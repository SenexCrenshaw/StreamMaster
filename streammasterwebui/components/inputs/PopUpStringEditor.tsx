import OKButton from '@components/buttons/OKButton';
import { OverlayPanel } from 'primereact/overlaypanel';
import { memo, useEffect, useRef, useState } from 'react';
import StringEditor from './StringEditor';
export interface PopUpStringEditorProperties {
  value: string | undefined;
  visible: boolean;
  onClose: (value: string | undefined) => void;
}

const PopUpStringEditor = ({ onClose, value, visible }: PopUpStringEditorProperties) => {
  const [input, setInput] = useState<string | undefined>(undefined);
  const [oldVisible, setOldVisible] = useState<boolean>(false);
  const anchorReference = useRef<HTMLDivElement | null>(null);

  const op = useRef<OverlayPanel>(null);

  useEffect(() => {
    if (value && value !== input) {
      setInput(value);
    }
  }, [value, input]);

  useEffect(() => {
    if (visible !== oldVisible) {
      setOldVisible(visible);
      if (visible) {
        op.current?.show(null, anchorReference.current);
      } else {
        op.current?.hide();
      }
    }
  }, [oldVisible, visible]);

  const ReturnToParent = (value?: string | undefined) => {
    setInput(value);
    setOldVisible(false);
    op.current?.hide();
    onClose(value);
  };

  return (
    <div ref={anchorReference}>
      <OverlayPanel ref={op} onHide={() => ReturnToParent()} style={{ width: '20vw' }}>
        <div className="flex grid p-0 m-0">
          <h4>Custom EPG Id</h4>
          <div className="flex col-12 p-0 m-0">
            <div className="col-10 p-0 m-0">
              <StringEditor
                autofocus={true}
                disableDebounce={true}
                value={input}
                onSave={(value) => {
                  setInput(value);
                  ReturnToParent(value);
                }}
              />
            </div>
            <div
              className="col-2 flex justify-content-end p-0 m-0"
              style={{
                height: 'var(--input-height)'
              }}
            >
              <OKButton
                iconFilled
                label={null}
                onClick={() => ReturnToParent(input)}
                style={{
                  width: 'var(--input-height)',
                  height: 'var(--input-height)'
                }}
              />
            </div>
          </div>
        </div>
      </OverlayPanel>
    </div>
  );
};

PopUpStringEditor.displayName = 'String Editor Body Template';

export default memo(PopUpStringEditor);
