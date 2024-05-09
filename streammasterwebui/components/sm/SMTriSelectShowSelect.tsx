import SMButton from '@components/sm/SMButton';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { useMemo } from 'react';

interface SMTriSelectShowSelectProperties {
  readonly id: string;
  readonly selectedItemsKey: string | undefined;

  onToggle: () => void;
}

export const SMTriSelectShowSelect = ({ onToggle, id, selectedItemsKey }: SMTriSelectShowSelectProperties) => {
  const { selectAll } = useSelectAll(id);
  const { selectedItems } = useSelectedItems<any>(selectedItemsKey ?? '');

  const selectAllStatus = useMemo(() => {
    let checked = selectAll ? true : selectedItems.length > 0 ? false : null;

    return checked;
  }, [selectAll, selectedItems.length]);

  const getToolTip = useMemo((): string => {
    if (selectAllStatus === true) {
      return 'All';
    }

    if (selectAllStatus === null) {
      return 'No Selection';
    }

    return `${selectedItems.length} items`;
  }, [selectAllStatus, selectedItems.length]);

  function toggleAllSelection() {
    onToggle();
  }

  const getIcon = useMemo(() => {
    if (selectAllStatus === true) {
      return 'pi-check-circle';
    }

    if (selectAllStatus === null) {
      return 'pi-circle';
    }

    return 'pi-verified';
  }, [selectAllStatus]);

  const getColor = useMemo(() => {
    if (selectAllStatus === true) {
      return 'icon-green';
    }

    if (selectAllStatus === null) {
      return 'icon-primary';
    }
    return 'icon-yellow';
  }, [selectAllStatus]);

  return <SMButton icon={getIcon} iconFilled={false} className={getColor} onClick={() => toggleAllSelection()} rounded tooltip={getToolTip} />;
};
