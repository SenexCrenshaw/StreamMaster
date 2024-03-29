import EPGSelector from '@components/epg/EPGSelector';

import { memo, useState } from 'react';

export interface StreamDataSelectorProperties {
  readonly data: VideoStreamDto;
  readonly enableEditMode?: boolean;
}

const TestPanel = () => {
  const [epg, setEpg] = useState<string | undefined>('5USA.uk');
  // const [iconSource, setIconSource] = useState<string | undefined>(undefined);
  return (
    <div className="col-5">
      <div className="">
        <EPGSelector
          enableEditMode
          onChange={(e: string) => {
            console.log(e);
          }}
          value={epg}
        />
      </div>
      {/* <div className="col-3">
        <IconSelector className="w-full bordered-text mr-2" onChange={setIconSource} value={iconSource} />
      </div> */}
    </div>
  );
};
export default memo(TestPanel);
