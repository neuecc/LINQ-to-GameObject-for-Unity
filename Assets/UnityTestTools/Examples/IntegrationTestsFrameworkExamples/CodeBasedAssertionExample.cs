using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTest;

[IntegrationTest.DynamicTestAttribute("ExampleIntegrationTests")]
[IntegrationTest.SucceedWithAssertions]
public class CodeBasedAssertionExample : MonoBehaviour
{
    public float FloatField = 3;

    public GameObject goReference;

    public void Awake()
    {
        // An assertion that will compare a foat value from a custom component attached to a GameObject to a constant variable equal to 3.
        // The comparasment will happen Start method and every 5 frames in the Update method
        // Additionally, the comparer is configured to have accuracy of 0.1 for floating euqlity check.
        IAssertionComponentConfigurator configurator;
        var c = AssertionComponent.Create<FloatComparer>(out configurator, CheckMethod.Update | CheckMethod.Start, gameObject, "CodeBasedAssertionExample.FloatField", 3f);
        configurator.UpdateCheckRepeatFrequency = 5;
        c.floatingPointError = 0.1;
        c.compareTypes = FloatComparer.CompareTypes.Equal;

        // Create an assertion that will fail is the FloatField from InitAssertions component of gameObject will change it's value
        AssertionComponent.Create<ValueDoesNotChange>(CheckMethod.Update | CheckMethod.Start, gameObject, "CodeBasedAssertionExample.FloatField");

        // Validate the gameObject.transform.y is always equal to 3 (defined in this component)
        transform.position = new Vector3(0, 3, 0);
        AssertionComponent.Create<FloatComparer>(CheckMethod.Update, gameObject, "CodeBasedAssertionExample.FloatField", gameObject, "transform.position.y");

        // Check with the goReference field from this component is not set to null
        goReference = gameObject;
        var gc = AssertionComponent.Create<GeneralComparer>(CheckMethod.Update, gameObject, "CodeBasedAssertionExample.goReference", null);
        gc.compareType = GeneralComparer.CompareType.ANotEqualsB;
    }
}
