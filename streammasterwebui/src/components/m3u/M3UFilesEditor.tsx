
import { memo } from "react";
import M3UFilesDataSelector from "./M3UFilesDataSelector";
import M3UFileDialog from "./M3UFileDialog";

const M3UFilesEditor = () => {

  return (
    <div className='m3uFilesEditor flex flex-column col-12 flex-shrink-0 '>
      <div className='flex justify-content-between align-items-center mb-1'>
        <span className='m-0 p-0 gap-1' style={{ color: '#FE7600' }}>M3U Files</span>
        <div className='m-0 p-0 flex gap-1'>
          <M3UFileDialog />
        </div>
      </div>

      <M3UFilesDataSelector />

    </div>
  );
}

M3UFilesEditor.displayName = 'M3UFilesEditor';

M3UFilesEditor.defaultProps = {

};

export type M3UFilesEditorProps = {

};


export default memo(M3UFilesEditor);
