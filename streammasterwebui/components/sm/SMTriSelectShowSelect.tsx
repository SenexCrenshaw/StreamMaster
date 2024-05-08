import SMButton from '@components/sm/SMButton';
import { useSelectAll } from '@lib/redux/slices/useSelectAll';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { useMemo } from 'react';

interface SMTriSelectShowSelectProperties {
  readonly id: string;
  readonly selectedItemsKey: string | undefined;

  onToggle: () => void;
}

export const SMTriSelectShowSelect = ({ onToggle, id, selectedItemsKey }: SMTriSelectShowSelectProperties) => {
  const { selectAll, setSelectAll } = useSelectAll(id);
  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems<any>(selectedItemsKey ?? '');

  const selectAllStatus = useMemo(() => {
    let checked = selectAll ? true : selectSelectedItems.length > 0 ? false : null;

    return checked;
  }, [selectAll, selectSelectedItems.length]);

  const getToolTip = useMemo((): string => {
    if (selectAllStatus === true) {
      return 'All';
    }

    if (selectAllStatus === null) {
      return 'No Selection';
    }

    return `${selectSelectedItems.length} items`;
  }, [selectAllStatus, selectSelectedItems.length]);

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
