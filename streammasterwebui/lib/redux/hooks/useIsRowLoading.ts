import { Logger } from '@lib/common/logger';
import { useAppDispatch, useAppSelector } from '../hooks';
import { RootState } from '../store';
import { selectIsRowLoading, setRowLoading } from './loading';

interface UseIsRowLoadingProps {
  Entity: string;
  Id: string;
}

const useIsRowLoading = ({ Entity, Id }: UseIsRowLoadingProps): [boolean, (isLoading: boolean) => void] => {
  const dispatch = useAppDispatch();
  const isRowLoading = useAppSelector((state: RootState) => selectIsRowLoading(state, Entity, Id));

  const setIsRowLoading = (isLoading: boolean) => {
    Logger.debug(`Setting row loading for ${Entity} ${Id} to ${isLoading}`);
    dispatch(setRowLoading({ entity: Entity, id: Id, isLoading }));
  };

  return [isRowLoading, setIsRowLoading];
};

export default useIsRowLoading;
