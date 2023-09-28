
import { memo } from "react";
import M3UFileDialog from "./M3UFileDialog";
import M3UFilesDataSelector from "./M3UFilesDataSelector";

const M3UFilesEditor = () => {

  return (
    <div className='m3uFilesEditor flex flex-column col-12'>
      <div className='flex justify-content-between align-items-center mb-1'>
        <span style={{ color: 'var(--orange-color)' }}>M3U Files</span>
        <div className='flex'>
          <M3UFileDialog />
        </div>
      </div>

      <M3UFilesDataSelector />

    </div>
  );
}

M3UFilesEditor.displayName = 'M3UFilesEditor';

export type M3UFilesEditorProps = {

};


export default memo(M3UFilesEditor);
