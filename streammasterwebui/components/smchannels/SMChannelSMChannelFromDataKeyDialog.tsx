import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import React from 'react';
import SMChannelSMChannelDataSelectorForDataKey from './SMChannelSMChannelDataSelectorForDataKey';
import SMChannelDataForSMChannelSelectorForDataKey from './SMChannelDataForSMChannelSelectorForDataKey';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';

interface SMChannelSMChannelFromDataKeyDialogProperties {
  readonly dataKey: string;
  readonly name?: string;
}

const SMChannelSMChannelFromDataKeyDialog = ({ dataKey, name }: SMChannelSMChannelFromDataKeyDialogProperties) => {
  // const [selectedItems, setSelectedItems] = useState<SMChannelDto[]>([]);
  const { selectedItems, setSelectedItems } = useSelectedItems<SMChannelDto>(dataKey);

  return (
    <div className="flex w-12 gap-1">
      <div className="w-6">
        <SMChannelSMChannelDataSelectorForDataKey
          height="250px"
          dataKey={dataKey}
          selectedItems={selectedItems}
          onChange={(e) => {
            setSelectedItems(e);
          }}
        />
      </div>
      <div className="w-6">
        <SMChannelDataForSMChannelSelectorForDataKey
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

SMChannelSMChannelFromDataKeyDialog.displayName = 'SMChannelSMChannelFromDataKeyDialog';

export default React.memo(SMChannelSMChannelFromDataKeyDialog);
