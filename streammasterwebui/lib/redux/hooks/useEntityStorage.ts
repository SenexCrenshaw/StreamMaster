import { useAppDispatch } from '../hooks';

interface EntityStorageResult<T> {
  setEntityValue: (value: T | undefined) => void;
  removeEntityValue: (key: string) => void;
}

function useEntityStorage<T>(
  key: string,
  slice: {
    actions: {
      setValue: (payload: { key: string; value: T | undefined }) => { type: string; payload: { key: string; value: T | undefined } };
      removeValue: (payload: string) => { type: string; payload: string };
    };
  }
): EntityStorageResult<T> {
  const dispatch = useAppDispatch();

  const setEntityValue = (value: T | undefined): void => {
    dispatch(slice.actions.setValue({ key: key, value: value }));
  };

  const removeEntityValue = (): void => {
    dispatch(slice.actions.removeValue(key));
  };

  return { setEntityValue, removeEntityValue };
}

export default useEntityStorage;
