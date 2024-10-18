import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import React from 'react';
import SMChannelSMStreamDataSelectorForDataKey from './SMChannelSMStreamDataSelectorForDataKey';
import SMStreamDataForSMChannelSelectorForDataKey from './SMStreamDataForSMChannelSelectorForDataKey';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
interface SMChannelSMStreamFromDataKeyDialogProperties {
  readonly dataKey: string;
  readonly name?: string;
}

const SMChannelSMStreamFromDataKeyDialog = ({ dataKey, name }: SMChannelSMStreamFromDataKeyDialogProperties) => {
  // const [selectedItems, setSelectedItems] = useState<SMStreamDto[]>([]);
  const { selectedItems, setSelectedItems } = useSelectedItems<SMStreamDto>(dataKey);
  return (
    <div className="flex w-12 gap-1">
      <div className="w-6">
        <SMChannelSMStreamDataSelectorForDataKey
          height="250px"
          dataKey={dataKey}
          selectedItems={selectedItems}
          onChange={(e) => {
            setSelectedItems(e);
          }}
        />
      </div>
      <div className="w-6">
        <SMStreamDataForSMChannelSelectorForDataKey
          height="250px"
          name={name}
          dataKey={dataKey}
          selectedItems={selectedItems}
          onChange={(e) => {
            setSelectedItems(e);
          }}
        />
      </div>
    </div>
  );
};

SMChannelSMStreamFromDataKeyDialog.displayName = 'SMChannelSMStreamFromDataKeyDialog';

export default React.memo(SMChannelSMStreamFromDataKeyDialog);
