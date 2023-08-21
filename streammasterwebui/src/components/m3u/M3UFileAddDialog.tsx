import { Button } from "primereact/button";
import { OverlayPanel } from "primereact/overlaypanel";
import { useRef, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { type M3UFileDto } from "../../store/iptvApi";
import M3UFilesEditor from "./M3UFilesEditor";

const M3UFileAddDialog = (props: M3UFilesEditorProps) => {

  const op = useRef<OverlayPanel>(null);

  return (
    <div className="flex justify-content-center align-items-center max-h-2rem" >
      <Button
        className="p-0 max-h-2rem "
        onClick={(e) => op?.current?.toggle(e)}
        size="small"
        style={{ backgroundColor: '#FE7600', border: 0 }}
        tooltip="M3U Files Selector"
        tooltipOptions={getTopToolOptions}
      >
        <img alt="logo" src="/images/m3u_iconx32.png"
          style={{
            height: '1.5rem',
          }}
        />
      </Button>
      <OverlayPanel
        className='col-7 p-0'
        ref={op}
        showCloseIcon={false}>
        <M3UFilesEditor
          onClick={(e) => props.onClick?.(e)}
          value={props.value}
        />
      </OverlayPanel>
    </div >
  );
}

M3UFileAddDialog.displayName = 'M3UFileAddDialog';
M3UFileAddDialog.defaultProps = {
};

export type M3UFilesEditorProps = {
  onClick?: (e: M3UFileDto) => void;
  value?: M3UFileDto | undefined;
};

export default memo(M3UFileAddDialog);
