import { useEffect, useRef } from "react";
import { useOnClickOutside, useBoolean } from 'usehooks-ts';
import useGetDetails, { Details, DetailsType } from "./get-details-store";

interface DetailsCardProps {
  id: number
  type: DetailsType
}

const ConnectionDetails = ({id, type}: DetailsCardProps) => {
  const getDetails = useGetDetails((state) => state.getDetails);
  const result = useGetDetails((state) => state.results);
  const { value, setTrue, setFalse } = useBoolean(false);

  useEffect(() => {
    setTrue();
    getDetails(id, type);
  }, [id, type]);

  const handleClickOutside = () => { setFalse(); };

  const renderAdditionalDetails = (details: Details) => {
    const props = Object.keys(details.additionalDetails);
    const list = props.map(p => ({
      label: p,
      text: details.additionalDetails[p]
    }));

    list.unshift({
      label: 'type',
      text: details.type
    });

    return (
      <>
        {list.map(item => (
          <div className='flex' key={item.label}>
            <label className='w-1/2
              capitalize
              font-semibold
              text-swblue dark:text-swyellow'>
              {item.label}
            </label>

            <span className='w-1/2
              capitalize
              text-black dark:text-white'>
              {item.text}
            </span>
          </div>
          )
        )}
      </>
    );
  };

  const renderIcon = (type: DetailsType) => {
    switch (type) {
      case DetailsType.Character:
        return (<i className='fa fa-user self-center'></i>);

      case DetailsType.Film:
        return (<i className='fa fa-film self-center'></i>);

      case DetailsType.Homeworld:
        return (<i className='fa fa-globe self-center'></i>)

      default:
        return null;
    }
  };

  const ref = useRef(null);
  useOnClickOutside(ref, handleClickOutside);

  return value && result && (
    <div
      ref={ref}
      className={`
        absolute top-2 left-2
        text-sm
        rounded-md
        border-2
        border-swblue dark:border-swyellow
        max-w-60 min-w-60
        bg-swgray/90 dark:bg-swblack/90
      `}
    >
      <div className='p-2
        font-semibold
        text-white dark:text-swblack
        bg-swblue dark:bg-swyellow
        flex justify-between items-stretch
      '>
        <span>{result.heading}</span>
        {renderIcon(type)}
      </div>
      <div className='p-2'>
        {renderAdditionalDetails(result)}
      </div>
    </div>
  );
};

export default ConnectionDetails;