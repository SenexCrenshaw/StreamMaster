import { Button } from "primereact/button";
import { OverlayPanel } from "primereact/overlaypanel";
import { useRef, memo } from "react";
import { getTopToolOptions } from "../../common/common";
import { type EpgFilesDto } from "../../store/iptvApi";
import EPGFilesEditor from "./EPGFilesEditor";

const EPGFileAddDialog = (props: EPGFilesEditorProps) => {
  const op = useRef<OverlayPanel>(null);

  // let EPGFileAddDialogurl = '/images/epg_iconx32.png'

  // if (isDev && EPGFileAddDialogurl && !EPGFileAddDialogurl.startsWith('http')) {
  //   EPGFileAddDialogurl = baseHostURL + EPGFileAddDialogurl;
  // }

  return (
    <div className="flex justify-content-center align-items-center max-h-2rem" >
      <Button
        className="p-0 max-h-2rem "

        onClick={(e) => op?.current?.toggle(e)}
        style={{ backgroundColor: '#FE7600', border: 0 }}
        tooltip="EPG Files Selector"
        tooltipOptions={getTopToolOptions}
      >

        <img alt="logo" src='/images/epg_iconx32.png'
          style={{
            height: '1.5rem',
          }}
        />
      </Button>
      <OverlayPanel
        className='col-6 p-0'
        ref={op}
        showCloseIcon={false}>
        <EPGFilesEditor
          onClick={(e) => props.onClick?.(e)}
          value={props.value}
        />
      </OverlayPanel>
    </div >
  );

}

EPGFileAddDialog.displayName = 'EPGFileAddDialog';
EPGFileAddDialog.defaultProps = {
};

export type EPGFilesEditorProps = {
  onClick?: (e: EpgFilesDto) => void;
  value?: EpgFilesDto | undefined;
};

export default memo(EPGFileAddDialog);
